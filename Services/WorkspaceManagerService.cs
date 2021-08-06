using ContestSystem.Extensions;
using ContestSystem.Models.DbContexts;
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
        private readonly ILogger<WorkspaceManagerService> _logger;
        private readonly FileStorageService _storage;

        public WorkspaceManagerService(ILogger<WorkspaceManagerService> logger, FileStorageService storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public async Task<ResponseObject<long>> CreateContestAsync(MainDbContext dbContext, ContestForm form, bool checkForLimit = false)
        {
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
                    _logger.LogCreationFailedBecauseOfLimits(Constants.ContestEntityName, form.CreatorUserId);
                    return ResponseObject<long>.Fail(Constants.ErrorContestCreationLimitExceeded);
                }

                contest.ApprovalStatus = ApproveType.NotModeratedYet;
            }
            else
            {
                contest.ApprovalStatus = ApproveType.Accepted;
            }

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
            for (int i = 0; i < form.Localizers.Count; i++)
            {
                var localizer = new ContestLocalizer
                {
                    Culture = form.Localizers[i].Culture,
                    Description = form.Localizers[i].Description,
                    Name = form.Localizers[i].Name,
                    ContestId = contest.Id
                };
                contest.ContestLocalizers.Add(localizer);
            }

            for (int i = 0; i < form.Problems.Count; i++)
            {
                var contestProblems = new ContestProblem
                {
                    ContestId = contest.Id,
                    ProblemId = form.Problems[i].ProblemId,
                    Letter = form.Problems[i].Letter
                };
                await dbContext.ContestsProblems.AddAsync(contestProblems);
            }

            await dbContext.SaveChangesAsync();
            if (contest.ApprovalStatus == ApproveType.Accepted)
            {
                _logger.LogCreationSuccessfulWithAutoAccept(Constants.ContestEntityName, contest.Id, form.CreatorUserId);
            }
            else
            {
                _logger.LogCreationSuccessful(Constants.ContestEntityName, contest.Id, form.CreatorUserId);
            }

            return ResponseObject<long>.Success(contest.Id);
        }

        public async Task<ResponseObject<long>> CreateProblemAsync(MainDbContext dbContext, ProblemForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> CreateCheckerAsync(MainDbContext dbContext, CheckerForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> CreateRulesSetAsync(MainDbContext dbContext, RulesSetForm form)
        {
            throw new NotImplementedException();
        }

        /*public async Task<ResponseObject<long>> CreateCourseAsync(MainDbContext dbContext, CourseForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ResponseObject<long>> CreatePostAsync(MainDbContext dbContext, PostForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> EditContestAsync(MainDbContext dbContext, ContestForm form, long userId, Contest contest = null)
        {
            contest ??= await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == form.Id.GetValueOrDefault(-1));
            if (contest == null)
            {
                _logger.LogEditingOfNonExistentEntity(Constants.ContestEntityName, form.Id.GetValueOrDefault(-1), userId);
                return ResponseObject<long>.Fail(Constants.ErrorContestDoesntExist);
            }

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

            /*var localizers = await dbContext.ContestsLocalizers.Where(l => l.ContestId == form.Id.Value).ToListAsync();
            var localizersExamined = new Dictionary<long, bool>();
            foreach (var l in localizers)
            {
                localizersExamined.Add(l.Id, false);
            }

            for (int i = 0; i < form.Localizers.Count; i++)
            {
                var localizer = new ContestLocalizer
                {
                    Culture = form.Localizers[i].Culture,
                    Description = form.Localizers[i].Description,
                    Name = form.Localizers[i].Name,
                    ContestId = contest.Id
                };
                var loadedLocalizer = localizers.FirstOrDefault(l => l.Culture == localizer.Culture);
                if (loadedLocalizer == null)
                {
                    await dbContext.ContestsLocalizers.AddAsync(localizer);
                }
                else
                {
                    localizersExamined[loadedLocalizer.Id] = true;
                    loadedLocalizer.Description = localizer.Description;
                    loadedLocalizer.Name = localizer.Name;
                    dbContext.ContestsLocalizers.Update(loadedLocalizer);
                }
            }

            foreach (var item in localizersExamined)
            {
                if (!item.Value)
                {
                    var loadedLocalizer = localizers.FirstOrDefault(l => l.Id == item.Key);
                    if (loadedLocalizer != null)
                    {
                        dbContext.ContestsLocalizers.Remove(loadedLocalizer);
                    }
                }
            }

            var problems = await dbContext.ContestsProblems.Where(cp => cp.ContestId == form.Id.Value).ToListAsync();
            var problemsExamined = new Dictionary<long, bool>();
            foreach (var p in problems)
            {
                problemsExamined.Add(p.ProblemId, false);
            }

            for (int i = 0; i < form.Problems.Count; i++)
            {
                var contestProblem = new ContestProblem
                {
                    ContestId = contest.Id,
                    ProblemId = form.Problems[i].ProblemId,
                    Letter = form.Problems[i].Letter
                };
                var loadedContestProblem =
                    problems.FirstOrDefault(cp => cp.ProblemId == contestProblem.ProblemId);
                if (loadedContestProblem == null)
                {
                    await dbContext.ContestsProblems.AddAsync(contestProblem);
                }
                else
                {
                    problemsExamined[loadedContestProblem.ProblemId] = true;
                    loadedContestProblem.Letter = form.Problems[i].Letter;
                    dbContext.ContestsProblems.Update(loadedContestProblem);
                }
            }

            foreach (var item in problemsExamined)
            {
                if (!item.Value)
                {
                    var loadedProblem = problems.FirstOrDefault(p => p.ProblemId == item.Key);
                    if (loadedProblem != null)
                    {
                        dbContext.ContestsProblems.Remove(loadedProblem);
                    }
                }
            }*/

            bool saveSuccess = await SecureEntitySaveAsync(dbContext);
            if (!saveSuccess)
            {
                _logger.LogParallelSaveError(Constants.ContestEntityName, form.Id.Value);
                return ResponseObject<long>.Fail(Constants.ErrorParallelDbSave);
            }

            _logger.LogEditingSuccessful(Constants.ContestEntityName, form.Id.Value, userId);
            return ResponseObject<long>.Success(form.Id.Value);
        }

        public async Task<ResponseObject<long>> EditProblemAsync(MainDbContext dbContext, ProblemForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> EditCheckerAsync(MainDbContext dbContext, CheckerForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> EditRulesSetAsync(MainDbContext dbContext, RulesSetForm form)
        {
            throw new NotImplementedException();
        }

        /*public async Task<ResponseObject<long>> EditCourseAsync(MainDbContext dbContext, CourseForm form)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ResponseObject<long>> EditPostAsync(MainDbContext dbContext, PostForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> DeleteContestAsync(MainDbContext dbContext, Contest contest, long userId)
        {
            if (contest == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(Constants.ContestEntityName, -1, userId);
                return ResponseObject<long>.Fail(Constants.ErrorContestDoesntExist);
            }
            long id = contest.Id;
            _storage.DeleteFileAsync(contest.ImagePath);
            dbContext.Contests.Remove(contest);
            bool saveSuccess = await SecureEntitySaveAsync(dbContext);
            if (!saveSuccess)
            {
                _logger.LogParallelSaveError(Constants.ContestEntityName, id, true);
                return ResponseObject<long>.Fail(Constants.ErrorParallelDbSave);
            }
            _logger.LogDeletingSuccessful(Constants.ContestEntityName, id, userId);
            return ResponseObject<long>.Success(id);
        }

        public async Task<ResponseObject<long>> DeleteProblemAsync(MainDbContext dbContext, ProblemForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> DeleteCheckerAsync(MainDbContext dbContext, CheckerForm form)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<long>> DeleteRulesSetAsync(MainDbContext dbContext, RulesSetForm form)
        {
            throw new NotImplementedException();
        }

        /*public async Task<ResponseObject<long>> DeleteCourseAsync(MainDbContext dbContext, CourseForm form)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ResponseObject<long>> DeletePostAsync(MainDbContext dbContext, PostForm form)
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

            return needToSave ? await SecureEntitySaveAsync(dbContext) : true;
        }

        private async Task<bool> SecureEntitySaveAsync(MainDbContext dbContext)
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}
