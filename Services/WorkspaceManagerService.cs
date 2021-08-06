using ContestSystem.Extensions;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestSystem.Services
{
    public class WorkspaceManagerService
    {
        private readonly CheckerSystemService _checkerSystem;
        private readonly FileStorageService _storage;

        public WorkspaceManagerService(CheckerSystemService checkerSystem, FileStorageService storage)
        {
            _checkerSystem = checkerSystem;
            _storage = storage;
        }

        public async Task<CreationStatusData> CreateContestAsync(MainDbContext dbContext, ContestForm form, bool checkForLimit = false)
        {
            var statusData = new CreationStatusData
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            Contest contest = new Contest
            {
                CreatorId = form.CreatorUserId,
                StartDateTimeUTC = form.StartDateTimeUTC,
                DurationInMinutes = form.DurationInMinutes,
                AreVirtualContestsAvailable = form.AreVirtualContestsAvailable,
                IsPublic = form.IsPublic,
                ContestLocalizers = new List<ContestLocalizer>(),
                RulesSetId = form.RulesSetId
            };

            if (checkForLimit)
            {
                if (await dbContext.Contests.CountAsync(c => c.CreatorId == form.CreatorUserId && c.ApprovalStatus == ApproveType.NotModeratedYet) >= Constants.ContestsLimitForLimitedUsers)
                {
                    statusData.Status = CreationStatus.LimitExceeded;
                }
                else
                {
                    contest.ApprovalStatus = ApproveType.NotModeratedYet;
                }
            }
            else
            {
                contest.ApprovalStatus = ApproveType.Accepted;
            }
            if (statusData.Status == CreationStatus.Undefined)
            {
                await dbContext.Contests.AddAsync(contest);
                await dbContext.SaveChangesAsync();
                contest.ImagePath = await _storage.SaveContestImageAsync(contest.Id, form.Image);
                dbContext.Contests.Update(contest);

                var localModerator = new ContestLocalModerator
                {
                    ContestId = contest.Id,
                    Alias = contest.Creator.FullName, // TODO: надо получать алиас из формы
                    LocalModeratorId = contest.CreatorId.GetValueOrDefault()
                };
                await dbContext.ContestsLocalModerators.AddAsync(localModerator);
                await dbContext.SaveChangesAsync();

                await CreateLinkedEntitiesAsync(dbContext, dbContext.ContestsLocalizers, contest.Id,
                                                form.Localizers,
                                                (clf, id) => new ContestLocalizer
                                                {
                                                    Culture = clf.Culture,
                                                    Description = clf.Description,
                                                    Name = clf.Name,
                                                    ContestId = id
                                                });

                await CreateLinkedEntitiesAsync(dbContext, dbContext.ContestsProblems, contest.Id,
                                               form.Problems,
                                               (cpf, id) => new ContestProblem
                                               {
                                                   ContestId = id,
                                                   ProblemId = cpf.ProblemId,
                                                   Letter = cpf.Letter
                                               });

                await dbContext.SaveChangesAsync();
                
                if (contest.ApprovalStatus == ApproveType.Accepted)
                {
                    statusData.Status = CreationStatus.SuccessWithAutoAccept;
                }
                else
                {
                    statusData.Status = CreationStatus.Success;
                }
                statusData.Id = contest.Id;
            }
            return statusData;
        }

        public async Task<CreationStatusData> CreateProblemAsync(MainDbContext dbContext, ProblemForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<CreationStatusData> CreateCheckerAsync(MainDbContext dbContext, CheckerForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<CreationStatusData> CreateRulesSetAsync(MainDbContext dbContext, RulesSetForm form)
        {
            throw new NotImplementedException();
        }

        /*public async Task<CreationStatusData> CreateCourseAsync(MainDbContext dbContext, CourseForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }*/

        public async Task<CreationStatusData> CreatePostAsync(MainDbContext dbContext, PostForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<EditionStatus> EditContestAsync(MainDbContext dbContext, ContestForm form, long userId, Contest contest = null)
        {
            var status = EditionStatus.Undefined;
            contest ??= await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == form.Id.GetValueOrDefault(-1));
            if (contest == null)
            {
                status = EditionStatus.NotExistentEntity;
            }
            else
            {
                if (form.Image != null)
                {
                    contest.ImagePath = await _storage.SaveContestImageAsync(form.Id.Value, form.Image);
                }

                contest.StartDateTimeUTC = form.StartDateTimeUTC;
                contest.DurationInMinutes = form.DurationInMinutes;
                contest.AreVirtualContestsAvailable = form.AreVirtualContestsAvailable;
                contest.IsPublic = form.IsPublic;
                contest.RulesSetId = form.RulesSetId;

                bool needToRemoderate = (form.StartDateTimeUTC != contest.StartDateTimeUTC && contest.Creator.IsLimitedInContests) || contest.ApprovalStatus == ApproveType.Rejected;
                if (needToRemoderate)
                {
                    contest.ApprovalStatus = ApproveType.NotModeratedYet;
                }

                dbContext.Contests.Update(contest);

                await UpdateLinkedEntitiesAsync(dbContext, dbContext.ContestsLocalizers, form.Localizers,
                                                                    l => l.ContestId == form.Id.Value,
                                                                    l => l.Culture,
                                                                    (id1, id2) => id1 == id2,
                                                                    lf => new ContestLocalizer
                                                                    {
                                                                        Culture = lf.Culture,
                                                                        Description = lf.Description,
                                                                        Name = lf.Name,
                                                                        ContestId = contest.Id
                                                                    },
                                                                    (l, lf) =>
                                                                    {
                                                                        ContestLocalizer localizer = l;
                                                                        localizer.Description = lf.Description;
                                                                        localizer.Name = localizer.Name;
                                                                        return localizer;
                                                                    });

                await UpdateLinkedEntitiesAsync(dbContext, dbContext.ContestsProblems, form.Problems,
                                                                cp => cp.ContestId == form.Id.Value,
                                                                cp => cp.ProblemId,
                                                                (id1, id2) => id1 == id2,
                                                                cpf => new ContestProblem
                                                                {
                                                                    ContestId = contest.Id,
                                                                    ProblemId = cpf.ProblemId,
                                                                    Letter = cpf.Letter
                                                                },
                                                                (cp, cpf) =>
                                                                {
                                                                    ContestProblem problem = cp;
                                                                    problem.Letter = cpf.Letter;
                                                                    return problem;
                                                                });

                bool saveSuccess = await SecureSaveAsync(dbContext);
                if (!saveSuccess)
                {
                    status = EditionStatus.ParallelSaveError;
                }
                else
                {
                    status = EditionStatus.Success;
                }
            }

            return status;
        }

        public async Task<EditionStatus> EditProblemAsync(MainDbContext dbContext, ProblemForm form, Problem problem = null)
        {
            throw new NotImplementedException();
        }

        public async Task<EditionStatus> EditCheckerAsync(MainDbContext dbContext, CheckerForm form, Checker checker = null)
        {
            throw new NotImplementedException();
        }

        public async Task<EditionStatus> EditRulesSetAsync(MainDbContext dbContext, RulesSetForm form, RulesSet rulesSet = null)
        {
            throw new NotImplementedException();
        }

        /*public async Task<EditionStatus> EditCourseAsync(MainDbContext dbContext, CourseForm form, Course course = null)
        {
            throw new NotImplementedException();
        }*/

        public async Task<EditionStatus> EditPostAsync(MainDbContext dbContext, PostForm form, Post post = null)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletionStatus> DeleteContestAsync(MainDbContext dbContext, Contest contest, long userId)
        {
            var status = DeletionStatus.Undefined;
            if (contest == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                _storage.DeleteFileAsync(contest.ImagePath);
                dbContext.Contests.Remove(contest);
                bool saveSuccess = await SecureSaveAsync(dbContext);
                if (!saveSuccess)
                {
                    status = DeletionStatus.ParallelSaveError;
                }
                else
                {
                    status = DeletionStatus.Success;
                }
            }
            
            return status;
        }

        public async Task<DeletionStatus> DeleteProblemAsync(MainDbContext dbContext, Problem problem, long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletionStatus> DeleteCheckerAsync(MainDbContext dbContext, Checker checker, long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletionStatus> DeleteRulesSetAsync(MainDbContext dbContext, RulesSet rulesSet, long userId)
        {
            throw new NotImplementedException();
        }

        /*public async Task<DeletionStatus> DeleteCourseAsync(MainDbContext dbContext, Course course, long userId)
        {
            throw new NotImplementedException();
        }*/

        public async Task<DeletionStatus> DeletePostAsync(MainDbContext dbContext, Post post, long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ModerationStatus> ModerateContestAsync(MainDbContext dbContext, ContestRequestForm form, Contest contest = null)
        {
            var status = ModerationStatus.Undefined;
            contest ??= await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == form.ContestId);
            if (contest == null)
            {
                status = ModerationStatus.NotExistentEntity;
            }
            else
            {
                contest.ApprovalStatus = form.ApprovalStatus;
                contest.ApprovingModeratorId = form.ApprovingModeratorId;
                contest.ModerationMessage = form.ModerationMessage;
                dbContext.Contests.Update(contest);

                bool saveSuccess = await SecureSaveAsync(dbContext);
                if (!saveSuccess)
                {
                    status = ModerationStatus.ParallelSaveError;
                }
                else
                {
                    status = (contest.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Approved : ModerationStatus.Rejected;
                }
            }
            return status;
        }

        public async Task<ModerationStatus> ModerateProblemAsync(MainDbContext dbContext, ProblemRequestForm form, Problem problem = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ModerationStatus> ModerateCheckerAsync(MainDbContext dbContext, CheckerRequestForm form, Checker checker = null)
        {
            throw new NotImplementedException();
        }

        /*public async Task<ModerationStatus> ModerateCourseAsync(MainDbContext dbContext, CourseRequestForm form, Course course = null)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ModerationStatus> ModeratePostAsync(MainDbContext dbContext, PostRequestForm form, Post post = null)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> UpdateLinkedEntitiesAsync<TEntityFromForm, TEntity, TLinkedIdentity>(MainDbContext dbContext, DbSet<TEntity> dbSet,
            List<TEntityFromForm> entitiesFromForm, Expression<Func<TEntity, bool>> predicateForDbSearch,
            Func<TEntity, TLinkedIdentity> predicateForLinkedEntityIdentity, Func<TLinkedIdentity, TLinkedIdentity, bool> predicateForLinkedIdentityCompare,
            Func<TEntityFromForm, TEntity> predicateForFormParse, Func<TEntity, TEntityFromForm, TEntity> predicateBeforeUpdate,
            bool needToSave = false)
            where TEntity : class
            where TEntityFromForm : class
            where TLinkedIdentity : IEquatable<TLinkedIdentity>
        {
            var entities = await dbSet.Where(predicateForDbSearch).ToListAsync();
            var entitiesExamined = new Dictionary<TLinkedIdentity, bool>();
            foreach (var entity in entities)
            {
                entitiesExamined.Add(predicateForLinkedEntityIdentity(entity), false);
            }

            for (int i = 0; i < entitiesFromForm.Count; i++)
            {
                var entity = predicateForFormParse(entitiesFromForm[i]);
                var loadedEntity = entities.FirstOrDefault(e => predicateForLinkedIdentityCompare(predicateForLinkedEntityIdentity(e), predicateForLinkedEntityIdentity(entity)));
                if (loadedEntity == null)
                {
                    await dbSet.AddAsync(entity);
                }
                else
                {
                    entitiesExamined[predicateForLinkedEntityIdentity(entity)] = true;
                    loadedEntity = predicateBeforeUpdate(loadedEntity, entitiesFromForm[i]);
                    dbSet.Update(loadedEntity);
                }
            }

            foreach (var item in entitiesExamined)
            {
                if (!item.Value)
                {
                    var loadedEntity = entities.FirstOrDefault(e => predicateForLinkedIdentityCompare(predicateForLinkedEntityIdentity(e), item.Key));
                    if (loadedEntity != null)
                    {
                        dbSet.Remove(loadedEntity);
                    }
                }
            }

            return needToSave ? await SecureSaveAsync(dbContext) : true;
        }

        private async Task<bool> CreateLinkedEntitiesAsync<TEntity, TForm, TIdentity>(MainDbContext dbContext, DbSet<TEntity> dbSet,
            TIdentity mainEntityId, List<TForm> entitiesFromForm, Func<TForm, TIdentity, TEntity> predicate, bool needToSave = false)
            where TEntity : class
            where TForm : class
        {
            for (int i = 0; i < entitiesFromForm.Count; i++)
            {
                var entity = predicate(entitiesFromForm[i], mainEntityId);
                await dbSet.AddAsync(entity);
            }

            return needToSave ? await SecureSaveAsync(dbContext) : true;
        }

        private async Task<bool> SecureSaveAsync(MainDbContext dbContext)
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }
    }
}
