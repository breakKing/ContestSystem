using ContestSystem.Areas.Contests.Services;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Workspace.Services
{
    public class WorkspaceManagerService
    {
        private readonly CheckerSystemService _checkerSystem;
        private readonly FileStorageService _storage;
        private readonly ContestsManagerService _contestsManager;

        private enum EntityInLockedContestStatus
        {
            NotLocked = 0,
            LockedByOwnContest = 1,
            LockedByOthersContests = 2,
            Undefined = 3
        }

        public WorkspaceManagerService(CheckerSystemService checkerSystem, FileStorageService storage, ContestsManagerService contestsManager)
        {
            _checkerSystem = checkerSystem;
            _storage = storage;
            _contestsManager = contestsManager;
        }

        public async Task<CreationStatusData<long?>> CreateContestAsync(MainDbContext dbContext, ContestForm form, bool checkForLimit = false)
        {
            var statusData = new CreationStatusData<long?>
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
                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    statusData.Status = CreationStatus.DbSaveError;
                }
                else
                {
                    contest.ImagePath = await _storage.SaveContestImageAsync(contest.Id, form.Image);
                    dbContext.Contests.Update(contest);

                    var organizer = new ContestOrganizer
                    {
                        ContestId = contest.Id,
                        Alias = contest.Creator.FullName, // TODO: надо получать алиас из формы
                        OrganizerId = contest.CreatorId.GetValueOrDefault(-1)
                    };
                    await dbContext.ContestsOrganizers.AddAsync(organizer);

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

                    saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = CreationStatus.DbSaveError;
                        contest = await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contest.Id);
                        await DeleteContestAsync(dbContext, contest);
                    }
                    else
                    {
                        if (contest.ApprovalStatus == ApproveType.Accepted)
                        {
                            statusData.Status = CreationStatus.SuccessWithAutoAccept;
                            await _contestsManager.CreateContestInitialChatsAsync(dbContext, contest);
                        }
                        else
                        {
                            statusData.Status = CreationStatus.Success;
                        }
                        statusData.Id = contest.Id;
                    }
                }
            }
            return statusData;
        }

        public async Task<CreationStatusData<long?>> CreateProblemAsync(MainDbContext dbContext, ProblemForm form, bool checkForLimit = false)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            var problem = new Problem
            {
                CreatorId = form.CreatorId,
                IsPublic = form.IsPublic,
                MemoryLimitInBytes = form.MemoryLimitInBytes,
                TimeLimitInMilliseconds = form.TimeLimitInMilliseconds,
                CheckerId = form.CheckerId,
                IsArchieved = false
            };

            if (checkForLimit)
            {
                if (await dbContext.Problems.CountAsync(p => p.CreatorId == form.CreatorId && p.ApprovalStatus == ApproveType.NotModeratedYet) >= Constants.ProblemsLimitForLimitedUsers)
                {
                    statusData.Status = CreationStatus.LimitExceeded;
                }
                else
                {
                    problem.ApprovalStatus = ApproveType.NotModeratedYet;
                }
            }
            else
            {
                problem.ApprovalStatus = ApproveType.Accepted;
            }

            if (statusData.Status == CreationStatus.Undefined)
            {
                await dbContext.Problems.AddAsync(problem);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    statusData.Status = CreationStatus.DbSaveError;
                }
                else
                {
                    await CreateLinkedEntitiesAsync(dbContext, dbContext.ProblemsLocalizers, problem.Id,
                                               form.Localizers,
                                               (lf, id) => new ProblemLocalizer
                                               {
                                                   Culture = lf.Culture,
                                                   Description = lf.Description,
                                                   Name = lf.Name,
                                                   InputBlock = lf.InputBlock,
                                                   OutputBlock = lf.OutputBlock,
                                                   ProblemId = id
                                               });

                    await CreateLinkedEntitiesAsync(dbContext, dbContext.Tests, problem.Id,
                                                   form.Tests,
                                                   (tf, id) => new Test
                                                   {
                                                       Number = tf.Number,
                                                       Input = tf.Input,
                                                       Answer = tf.Answer,
                                                       AvailablePoints = tf.AvailablePoints,
                                                       ProblemId = id
                                                   });

                    await CreateLinkedEntitiesAsync(dbContext, dbContext.Examples, problem.Id,
                                                  form.Examples,
                                                  (ef, id) => new Example
                                                  {
                                                      Number = ef.Number,
                                                      InputText = ef.InputText,
                                                      OutputText = ef.OutputText,
                                                      ProblemId = id
                                                  });

                    saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = CreationStatus.DbSaveError;
                        problem = await dbContext.Problems.FirstOrDefaultAsync(p => p.Id == problem.Id);
                        await DeleteProblemAsync(dbContext, problem);
                    }
                    else
                    {
                        if (problem.ApprovalStatus == ApproveType.Accepted)
                        {
                            statusData.Status = CreationStatus.SuccessWithAutoAccept;
                        }
                        else
                        {
                            statusData.Status = CreationStatus.Success;
                        }
                        statusData.Id = problem.Id;
                    }
                }
            }

            return statusData;
        }

        public async Task<CreationStatusData<long?>> CreateCheckerAsync(MainDbContext dbContext, CheckerForm form)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            var checker = new Checker
            {
                AuthorId = form.AuthorId,
                Code = form.Code,
                Name = form.Name,
                Description = form.Description,
                IsPublic = form.IsPublic,
                IsArchieved = false
            };
            checker.ApprovalStatus = ApproveType.NotModeratedYet;
            await dbContext.Checkers.AddAsync(checker);

            bool saveSuccess = await dbContext.SecureSaveAsync();
            if (!saveSuccess)
            {
                statusData.Status = CreationStatus.DbSaveError;
            }
            else
            {
                statusData.Status = CreationStatus.Success;
                statusData.Id = checker.Id;
            }

            return statusData;
        }

        public async Task<CreationStatusData<long?>> CreateRulesSetAsync(MainDbContext dbContext, RulesSetForm form, bool checkForLimit = false)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            var rulesSet = new RulesSet
            {
                Name = form.Name,
                Description = form.Description,
                ShowFullTestsResults = form.ShowFullTestsResults,
                PointsForBestSolution = form.PointsForBestSolution,
                CountMode = form.CountMode,
                MaxTriesForOneProblem = form.MaxTriesForOneProblem,
                PenaltyForOneMinute = form.PenaltyForOneMinute,
                MonitorFreezeTimeBeforeFinishInMinutes = form.MonitorFreezeTimeBeforeFinishInMinutes,
                PenaltyForCompilationError = form.PenaltyForCompilationError,
                PenaltyForOneTry = form.PenaltyForOneTry,
                PublicMonitor = form.PublicMonitor,
                AuthorId = form.AuthorId,
                IsPublic = form.IsPublic,
                IsArchieved = false
            };

            if (checkForLimit)
            {
                if (await dbContext.RulesSets.CountAsync(rs => rs.AuthorId == form.AuthorId && rs.ApprovalStatus == ApproveType.NotModeratedYet) >= Constants.RulesSetsLimitForLimitedUsers)
                {
                    statusData.Status = CreationStatus.LimitExceeded;
                }
                else
                {
                    rulesSet.ApprovalStatus = ApproveType.NotModeratedYet;
                }
            }
            else
            {
                rulesSet.ApprovalStatus = ApproveType.Accepted;
            }

            await dbContext.RulesSets.AddAsync(rulesSet);

            bool saveSuccess = await dbContext.SecureSaveAsync();
            if (!saveSuccess)
            {
                statusData.Status = CreationStatus.DbSaveError;
            }
            else
            {
                statusData.Status = CreationStatus.Success;
                statusData.Id = rulesSet.Id;
            }

            return statusData;
        }

        /*public async Task<CreationStatusData> CreateCourseAsync(MainDbContext dbContext, CourseForm form, bool checkForLimit = false)
        {
            throw new NotImplementedException();
        }*/

        public async Task<CreationStatusData<long?>> CreatePostAsync(MainDbContext dbContext, PostForm form, bool checkForLimit = false)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            Post post = new Post
            {
                PromotedDateTimeUTC = DateTime.UtcNow,
                AuthorId = form.AuthorUserId,
                PostLocalizers = new List<PostLocalizer>()
            };

            if (checkForLimit)
            {
                if (await dbContext.Posts.CountAsync(p => p.AuthorId == form.AuthorUserId && p.ApprovalStatus == ApproveType.NotModeratedYet) >= Constants.PostsLimitForLimitedUsers)
                {
                    statusData.Status = CreationStatus.LimitExceeded;
                }
                else
                {
                    post.ApprovalStatus = ApproveType.NotModeratedYet;
                }
            }
            else
            {
                post.ApprovalStatus = ApproveType.Accepted;
                post.PublicationDateTimeUTC = DateTime.UtcNow;
            }
            if (statusData.Status == CreationStatus.Undefined)
            {
                await dbContext.Posts.AddAsync(post);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    statusData.Status = CreationStatus.DbSaveError;
                }
                else
                {
                    post.ImagePath = await _storage.SavePostImageAsync(post.Id, form.PreviewImage);
                    dbContext.Posts.Update(post);

                    await CreateLinkedEntitiesAsync(dbContext, dbContext.PostsLocalizers, post.Id,
                                                    form.Localizers,
                                                    (plf, id) => new PostLocalizer
                                                    {
                                                        Culture = plf.Culture,
                                                        PreviewText = plf.PreviewText,
                                                        Name = plf.Name,
                                                        HtmlText = plf.HtmlText,
                                                        PostId = id
                                                    });

                    saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = CreationStatus.DbSaveError;
                        post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);
                        await DeletePostAsync(dbContext, post);
                    }
                    else
                    {
                        if (post.ApprovalStatus == ApproveType.Accepted)
                        {
                            statusData.Status = CreationStatus.SuccessWithAutoAccept;
                        }
                        else
                        {
                            statusData.Status = CreationStatus.Success;
                        }
                        statusData.Id = post.Id;
                    }
                }
            }
            return statusData;
        }

        public async Task<EditionStatus> EditContestAsync(MainDbContext dbContext, ContestForm form, Contest contest = null)
        {
            var status = EditionStatus.Undefined;
            contest ??= await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == form.Id.GetValueOrDefault(-1));
            if (contest == null)
            {
                status = EditionStatus.NotExistentEntity;
            }
            else
            {
                if (IsContestLocked(contest))
                {
                    status = EditionStatus.ContestLocked;
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
                                                        localizer.Name = lf.Name;
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

                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        status = EditionStatus.Success;
                    }
                }
            }

            return status;
        }

        public async Task<EditionStatusData> EditProblemAsync(MainDbContext dbContext, ProblemForm form, Problem problem = null)
        {
            var statusData = new EditionStatusData
            {
                Status = EditionStatus.Undefined,
                Id = null
            };

            problem ??= await dbContext.Problems.FirstOrDefaultAsync(c => c.Id == form.Id.GetValueOrDefault(-1));
            if (problem == null)
            {
                statusData.Status = EditionStatus.NotExistentEntity;
            }
            else
            {
                var lockStatus = await IsEntityUsedInLockedContestsAsync(dbContext, form.CreatorId,
                                                                                c => c.ContestProblems.Any(cp => cp.ProblemId == problem.Id));
                if (lockStatus == EntityInLockedContestStatus.LockedByOwnContest)
                {
                    statusData.Status = EditionStatus.ContestLocked;
                }
                else if (lockStatus == EntityInLockedContestStatus.LockedByOthersContests)
                {
                    var creationStatusData = await CreateProblemAsync(dbContext, form);

                    if (creationStatusData.Status == CreationStatus.DbSaveError)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        var newProblem = await dbContext.Problems.FirstOrDefaultAsync(p => p.Id == creationStatusData.Id.GetValueOrDefault(-1));
                        if (newProblem == null)
                        {
                            statusData.Status = EditionStatus.DbSaveError;
                        }
                        else
                        {
                            newProblem.ApprovalStatus = problem.ApprovalStatus;
                            if (newProblem.ApprovalStatus == ApproveType.Rejected)
                            {
                                newProblem.ApprovalStatus = ApproveType.NotModeratedYet;
                            }
                            newProblem.ApprovingModeratorId = problem.ApprovingModeratorId;
                            newProblem.ModerationMessage = problem.ModerationMessage;

                            dbContext.Problems.Update(newProblem);

                            bool saveSuccess = await dbContext.SecureSaveAsync();
                            if (!saveSuccess)
                            {
                                newProblem = await dbContext.Problems.FirstOrDefaultAsync(p => p.Id == creationStatusData.Id.GetValueOrDefault(-1));
                                if (newProblem != null)
                                {
                                    await DeleteProblemAsync(dbContext, newProblem);
                                }

                                statusData.Status = EditionStatus.DbSaveError;
                            }
                            else
                            {
                                await DeleteProblemAsync(dbContext, problem, true);
                                statusData.Status = EditionStatus.ArchivedAndRecreated;
                                statusData.Id = newProblem.Id;
                            }
                        }
                    }
                }
                else
                {
                    problem.MemoryLimitInBytes = form.MemoryLimitInBytes;
                    problem.TimeLimitInMilliseconds = form.TimeLimitInMilliseconds;
                    problem.IsPublic = form.IsPublic;
                    problem.CheckerId = form.CheckerId;
                    if (problem.ApprovalStatus == ApproveType.Rejected)
                    {
                        problem.ApprovalStatus = ApproveType.NotModeratedYet;
                    }

                    dbContext.Problems.Update(problem);

                    await UpdateLinkedEntitiesAsync(dbContext, dbContext.ProblemsLocalizers, form.Localizers,
                                                    l => l.ProblemId == form.Id.Value,
                                                    l => l.Culture,
                                                    (id1, id2) => id1 == id2,
                                                    lf => new ProblemLocalizer
                                                    {
                                                        Culture = lf.Culture,
                                                        Description = lf.Description,
                                                        InputBlock = lf.InputBlock,
                                                        OutputBlock = lf.OutputBlock,
                                                        Name = lf.Name,
                                                        ProblemId = problem.Id
                                                    },
                                                    (l, lf) =>
                                                    {
                                                        ProblemLocalizer localizer = l;
                                                        localizer.Description = lf.Description;
                                                        localizer.Name = lf.Name;
                                                        localizer.InputBlock = lf.InputBlock;
                                                        localizer.OutputBlock = lf.OutputBlock;
                                                        return localizer;
                                                    });

                    await UpdateLinkedEntitiesAsync(dbContext, dbContext.Tests, form.Tests,
                                                    t => t.ProblemId == form.Id.Value,
                                                    t => t.Number,
                                                    (id1, id2) => id1 == id2,
                                                    tf => new Test
                                                    {
                                                        Number = tf.Number,
                                                        Input = tf.Input,
                                                        Answer = tf.Answer,
                                                        AvailablePoints = tf.AvailablePoints,
                                                        ProblemId = problem.Id
                                                    },
                                                    (t, tf) =>
                                                    {
                                                        Test test = t;
                                                        test.AvailablePoints = tf.AvailablePoints;
                                                        test.Input = tf.Input;
                                                        test.Answer = tf.Answer;
                                                        return test;
                                                    });

                    await UpdateLinkedEntitiesAsync(dbContext, dbContext.Examples, form.Examples,
                                                    ex => ex.ProblemId == form.Id.Value,
                                                    ex => ex.Number,
                                                    (id1, id2) => id1 == id2,
                                                    ef => new Example
                                                    {
                                                        Number = ef.Number,
                                                        InputText = ef.InputText,
                                                        OutputText = ef.OutputText,
                                                        ProblemId = problem.Id
                                                    },
                                                    (e, ef) =>
                                                    {
                                                        Example example = e;
                                                        example.InputText = ef.InputText;
                                                        example.OutputText = ef.OutputText;
                                                        return example;
                                                    });

                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        statusData.Status = EditionStatus.Success;
                        statusData.Id = problem.Id;
                    }
                }
            }

            return statusData;
        }

        public async Task<EditionStatusData> EditCheckerAsync(MainDbContext dbContext, CheckerForm form, Checker checker = null)
        {
            var statusData = new EditionStatusData
            {
                Status = EditionStatus.Undefined,
                Id = null
            };

            checker ??= await dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == form.Id.GetValueOrDefault(-1) && !ch.IsArchieved);
            if (checker == null)
            {
                statusData.Status = EditionStatus.NotExistentEntity;
            }
            else
            {
                var lockStatus = await IsEntityUsedInLockedContestsAsync(dbContext, form.AuthorId,
                                                                                c => c.ContestProblems.Any(cp => cp.Problem.CheckerId == checker.Id));
                if (lockStatus == EntityInLockedContestStatus.LockedByOwnContest)
                {
                    statusData.Status = EditionStatus.ContestLocked;
                }
                else if (lockStatus == EntityInLockedContestStatus.LockedByOthersContests)
                {
                    var creationStatusData = await CreateCheckerAsync(dbContext, form);

                    if (creationStatusData.Status == CreationStatus.DbSaveError)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        var newChecker = await dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == creationStatusData.Id.GetValueOrDefault(-1));
                        if (newChecker == null)
                        {
                            statusData.Status = EditionStatus.DbSaveError;
                        }
                        else
                        {
                            bool needToRecompile = (checker.Code != form.Code) || checker.ApprovalStatus == ApproveType.Rejected;
                            if (needToRecompile)
                            {
                                newChecker.ApprovalStatus = ApproveType.NotModeratedYet;
                            }
                            else
                            {
                                newChecker.ApprovalStatus = checker.ApprovalStatus;
                            }

                            if (newChecker.ApprovalStatus == ApproveType.Accepted)
                            {
                                var requestForm = new CheckerRequestForm
                                {
                                    CheckerId = newChecker.Id,
                                    ApprovalStatus = ApproveType.Accepted,
                                    ApprovingModeratorId = checker.ApprovingModeratorId.Value,
                                    ModerationMessage = checker.ModerationMessage
                                };

                                var moderationStatus = await ModerateCheckerAsync(dbContext, requestForm, newChecker);
                                if (moderationStatus == ModerationStatus.DbSaveError)
                                {
                                    newChecker = await dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == creationStatusData.Id.GetValueOrDefault(-1));
                                    if (newChecker != null)
                                    {
                                        await DeleteCheckerAsync(dbContext, newChecker);
                                    }

                                    statusData.Status = EditionStatus.DbSaveError;
                                }
                            }
                            else
                            {
                                newChecker.ApprovingModeratorId = checker.ApprovingModeratorId;
                                newChecker.ModerationMessage = checker.ModerationMessage;

                                dbContext.Checkers.Update(newChecker);

                                bool saveSuccess = await dbContext.SecureSaveAsync();
                                if (!saveSuccess)
                                {
                                    newChecker = await dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == creationStatusData.Id.GetValueOrDefault(-1));
                                    if (newChecker != null)
                                    {
                                        await DeleteCheckerAsync(dbContext, newChecker);
                                    }

                                    statusData.Status = EditionStatus.DbSaveError;
                                }
                            }

                            if (statusData.Status != EditionStatus.DbSaveError)
                            {
                                await DeleteCheckerAsync(dbContext, checker, true);
                                statusData.Status = EditionStatus.ArchivedAndRecreated;
                                statusData.Id = newChecker.Id;
                            }
                        }
                    }
                }
                else
                {
                    checker.Code = form.Code;
                    checker.Name = form.Name;
                    checker.Description = form.Description;
                    checker.IsPublic = form.IsPublic;

                    bool needToRecompile = (checker.Code != form.Code) || checker.ApprovalStatus == ApproveType.Rejected;
                    if (needToRecompile)
                    {
                        checker.ApprovalStatus = ApproveType.NotModeratedYet;
                    }

                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        statusData.Status = EditionStatus.Success;
                        statusData.Id = checker.Id;
                    }
                }
            }
            return statusData;
        }

        public async Task<EditionStatusData> EditRulesSetAsync(MainDbContext dbContext, RulesSetForm form, RulesSet rulesSet = null)
        {
            var statusData = new EditionStatusData
            {
                Status = EditionStatus.Undefined,
                Id = null
            };

            rulesSet ??= await dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == form.Id.GetValueOrDefault(-1) && !rs.IsArchieved);
            if (rulesSet == null)
            {
                statusData.Status = EditionStatus.NotExistentEntity;
            }
            else
            {
                var lockStatus = await IsEntityUsedInLockedContestsAsync(dbContext, form.AuthorId,
                                                                                c => c.RulesSetId == rulesSet.Id);
                if (lockStatus == EntityInLockedContestStatus.LockedByOwnContest)
                {
                    statusData.Status = EditionStatus.ContestLocked;
                }
                else if (lockStatus == EntityInLockedContestStatus.LockedByOthersContests)
                {
                    var creationStatusData = await CreateRulesSetAsync(dbContext, form);

                    if (creationStatusData.Status == CreationStatus.DbSaveError)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        var newRulesSet = await dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == creationStatusData.Id.GetValueOrDefault(-1));
                        if (newRulesSet == null)
                        {
                            statusData.Status = EditionStatus.DbSaveError;
                        }
                        else
                        {
                            await DeleteRulesSetAsync(dbContext, rulesSet, true);
                            statusData.Status = EditionStatus.ArchivedAndRecreated;
                            statusData.Id = newRulesSet.Id;
                        }
                    }
                }
                else
                {
                    rulesSet.Name = form.Name;
                    rulesSet.Description = form.Description;
                    rulesSet.ShowFullTestsResults = form.ShowFullTestsResults;
                    rulesSet.PointsForBestSolution = form.PointsForBestSolution;
                    rulesSet.CountMode = form.CountMode;
                    rulesSet.MaxTriesForOneProblem = form.MaxTriesForOneProblem;
                    rulesSet.PenaltyForOneMinute = form.PenaltyForOneMinute;
                    rulesSet.MonitorFreezeTimeBeforeFinishInMinutes = form.MonitorFreezeTimeBeforeFinishInMinutes;
                    rulesSet.PenaltyForCompilationError = form.PenaltyForCompilationError;
                    rulesSet.PenaltyForOneTry = form.PenaltyForOneTry;
                    rulesSet.PublicMonitor = form.PublicMonitor;
                    rulesSet.IsPublic = form.IsPublic;

                    dbContext.RulesSets.Update(rulesSet);

                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        statusData.Status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        statusData.Status = EditionStatus.Success;
                        statusData.Id = rulesSet.Id;
                    }
                }
            }
            return statusData;
        }

        /*public async Task<EditionStatus> EditCourseAsync(MainDbContext dbContext, CourseForm form, Course course = null)
        {
            throw new NotImplementedException();
        }*/

        public async Task<EditionStatus> EditPostAsync(MainDbContext dbContext, PostForm form, Post post = null)
        {
            var status = EditionStatus.Undefined;

            post ??= await dbContext.Posts.FirstOrDefaultAsync(c => c.Id == form.Id.GetValueOrDefault(-1));
            if (post == null)
            {
                status = EditionStatus.NotExistentEntity;
            }
            else
            {
                if (form.PreviewImage != null)
                {
                    post.ImagePath = await _storage.SavePostImageAsync(form.Id.Value, form.PreviewImage);
                }

                if (post.ApprovalStatus == ApproveType.Rejected)
                {
                    post.ApprovalStatus = ApproveType.NotModeratedYet;
                    post.ApprovingModeratorId = null;
                }
                else if (post.ApprovalStatus == ApproveType.Accepted)
                {
                    post.PublicationDateTimeUTC = DateTime.UtcNow;
                }

                dbContext.Posts.Update(post);

                await UpdateLinkedEntitiesAsync(dbContext, dbContext.PostsLocalizers, form.Localizers,
                                                l => l.PostId == form.Id.Value,
                                                l => l.Culture,
                                                (id1, id2) => id1 == id2,
                                                lf => new PostLocalizer
                                                {
                                                    Culture = lf.Culture,
                                                    PreviewText = lf.PreviewText,
                                                    Name = lf.Name,
                                                    HtmlText = lf.HtmlText,
                                                    PostId = post.Id
                                                },
                                                (l, lf) =>
                                                {
                                                    PostLocalizer localizer = l;
                                                    localizer.PreviewText = lf.PreviewText;
                                                    localizer.Name = lf.Name;
                                                    localizer.HtmlText = lf.HtmlText;
                                                    return localizer;
                                                });

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = EditionStatus.DbSaveError;
                }
                else
                {
                    status = EditionStatus.Success;
                }
            }

            return status;
        }

        public async Task<DeletionStatus> DeleteContestAsync(MainDbContext dbContext, Contest contest)
        {
            var status = DeletionStatus.Undefined;
            if (contest == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                _storage.DeleteFileAsync(contest.ImagePath);
                await _contestsManager.RemoveAllContestChatsAsync(dbContext, contest.Id);
                dbContext.Contests.Remove(contest);
                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = DeletionStatus.DbSaveError;
                }
                else
                {
                    status = DeletionStatus.Success;
                }
            }
            return status;
        }

        public async Task<DeletionStatus> DeleteProblemAsync(MainDbContext dbContext, Problem problem, bool forceArchiving = false)
        {
            var status = DeletionStatus.Undefined;
            if (problem == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                if (forceArchiving || await dbContext.ContestsProblems.AnyAsync(cp => cp.ProblemId == problem.Id) || await dbContext.CoursesProblems.AnyAsync(cp => cp.ProblemId == problem.Id))
                {
                    do
                    {
                        problem.IsArchieved = true;
                        dbContext.Problems.Update(problem);
                    }
                    while (!await dbContext.SecureSaveAsync() || (problem = await dbContext.Problems.FirstOrDefaultAsync(p => p.Id == problem.Id && !p.IsArchieved)) != null);
                    status = DeletionStatus.SuccessWithArchiving;
                }
                else
                {
                    dbContext.Problems.Remove(problem);
                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        status = DeletionStatus.DbSaveError;
                    }
                    else
                    {
                        status = DeletionStatus.Success;
                    }
                }
            }
            return status;
        }

        public async Task<DeletionStatus> DeleteCheckerAsync(MainDbContext dbContext, Checker checker, bool forceArchiving = false)
        {
            var status = DeletionStatus.Undefined;
            if (checker == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                if (forceArchiving || await dbContext.Problems.AnyAsync(p => p.CheckerId == checker.Id))
                {
                    do
                    {
                        checker.IsArchieved = true;
                        dbContext.Checkers.Update(checker);
                    }
                    while (!await dbContext.SecureSaveAsync() || (checker = await dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == checker.Id && !ch.IsArchieved)) != null);
                    status = DeletionStatus.SuccessWithArchiving;
                }
                else
                {
                    dbContext.Checkers.Remove(checker);
                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        status = DeletionStatus.DbSaveError;
                    }
                    else
                    {
                        status = DeletionStatus.Success;
                    }
                }
            }
            return status;
        }

        public async Task<DeletionStatus> DeleteRulesSetAsync(MainDbContext dbContext, RulesSet rulesSet, bool forceArchiving = false)
        {
            var status = DeletionStatus.Undefined;
            if (rulesSet == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                if (forceArchiving || await dbContext.Contests.AnyAsync(c => c.RulesSetId == rulesSet.Id))
                {
                    do
                    {
                        rulesSet.IsArchieved = true;
                        dbContext.RulesSets.Update(rulesSet);
                    }
                    while (!await dbContext.SecureSaveAsync() || (rulesSet = await dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == rulesSet.Id && !rs.IsArchieved)) != null);
                    status = DeletionStatus.SuccessWithArchiving;
                }
                else
                {
                    dbContext.RulesSets.Remove(rulesSet);
                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        status = DeletionStatus.DbSaveError;
                    }
                    else
                    {
                        status = DeletionStatus.Success;
                    }
                }
            }
            return status;
        }

        /*public async Task<DeletionStatus> DeleteCourseAsync(MainDbContext dbContext, Course course, long userId)
        {
            throw new NotImplementedException();
        }*/

        public async Task<DeletionStatus> DeletePostAsync(MainDbContext dbContext, Post post)
        {
            var status = DeletionStatus.Undefined;
            if (post == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                _storage.DeleteFileAsync(post.ImagePath);
                dbContext.Posts.Remove(post);
                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = DeletionStatus.DbSaveError;
                }
                else
                {
                    status = DeletionStatus.Success;
                }
            }

            return status;
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

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = ModerationStatus.DbSaveError;
                }
                else
                {
                    status = (contest.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Accepted : ModerationStatus.Rejected;

                    if (!await _contestsManager.InitialChatsExistAsync(dbContext, contest.Id))
                    {
                        await _contestsManager.CreateContestInitialChatsAsync(dbContext, contest);
                    }
                }
            }
            return status;
        }

        public async Task<ModerationStatus> ModerateProblemAsync(MainDbContext dbContext, ProblemRequestForm form, Problem problem = null)
        {
            var status = ModerationStatus.Undefined;
            problem ??= await dbContext.Problems.FirstOrDefaultAsync(p => p.Id == form.ProblemId);
            if (problem == null)
            {
                status = ModerationStatus.NotExistentEntity;
            }
            else
            {
                problem.ApprovalStatus = form.ApprovalStatus;
                problem.ApprovingModeratorId = form.ApprovingModeratorId;
                problem.ModerationMessage = form.ModerationMessage;
                dbContext.Problems.Update(problem);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = ModerationStatus.DbSaveError;
                }
                else
                {
                    status = (problem.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Accepted : ModerationStatus.Rejected;
                }
            }
            return status;
        }

        public async Task<ModerationStatus> ModerateCheckerAsync(MainDbContext dbContext, CheckerRequestForm form, Checker checker = null)
        {
            var status = ModerationStatus.Undefined;

            checker ??= await dbContext.Checkers.FirstOrDefaultAsync(c => c.Id == form.CheckerId);
            if (checker == null)
            {
                status = ModerationStatus.NotExistentEntity;
            }
            else
            {
                checker.ApprovalStatus = form.ApprovalStatus;
                checker.ApprovingModeratorId = form.ApprovingModeratorId;
                checker.ModerationMessage = form.ModerationMessage;

                if (checker.ApprovalStatus == ApproveType.Accepted)
                {
                    var newChecker = await _checkerSystem.SendCheckerForCompilationAsync(dbContext, checker);
                    if (newChecker == null)
                    {
                        checker.CompilationVerdict = VerdictType.CheckerServersUnavailable;
                        checker.ApprovalStatus = ApproveType.Rejected;
                    }
                    else
                    {
                        checker.CompilationVerdict = newChecker.CompilationVerdict;
                        checker.Errors = newChecker.Errors;

                        if (newChecker.CompilationVerdict != VerdictType.CompilationSucceed)
                        {
                            checker.ApprovalStatus = ApproveType.Rejected;
                            checker.ModerationMessage = "Compilation errors:\n" + newChecker.Errors;
                            status = ModerationStatus.Rejected;
                        }
                        else
                        {
                            status = ModerationStatus.Accepted;
                        }
                    }
                }

                dbContext.Checkers.Update(checker);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = ModerationStatus.DbSaveError;
                }
                else
                {
                    status = (checker.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Accepted : ModerationStatus.Rejected;
                }
            }

            return status;
        }

        public async Task<ModerationStatus> ModerateRulesSetAsync(MainDbContext dbContext, RulesSetRequestForm form, RulesSet rulesSet = null)
        {
            var status = ModerationStatus.Undefined;
            rulesSet ??= await dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == form.RulesSetId);
            if (rulesSet == null)
            {
                status = ModerationStatus.NotExistentEntity;
            }
            else
            {
                rulesSet.ApprovalStatus = form.ApprovalStatus;
                rulesSet.ApprovingModeratorId = form.ApprovingModeratorId;
                rulesSet.ModerationMessage = form.ModerationMessage;
                dbContext.RulesSets.Update(rulesSet);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = ModerationStatus.DbSaveError;
                }
                else
                {
                    status = (rulesSet.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Accepted : ModerationStatus.Rejected;
                }
            }
            return status;
        }

        /*public async Task<ModerationStatus> ModerateCourseAsync(MainDbContext dbContext, CourseRequestForm form, Course course = null)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ModerationStatus> ModeratePostAsync(MainDbContext dbContext, PostRequestForm form, Post post = null)
        {
            var status = ModerationStatus.Undefined;

            post ??= await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == form.PostId);
            if (post == null)
            {
                status = ModerationStatus.NotExistentEntity;
            }
            else
            {
                post.ApprovalStatus = form.ApprovalStatus;
                post.ApprovingModeratorId = form.ApprovingModeratorId;
                post.ModerationMessage = form.ModerationMessage;
                post.PublicationDateTimeUTC = DateTime.UtcNow;

                dbContext.Posts.Update(post);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = ModerationStatus.DbSaveError;
                }
                else
                {
                    status = (post.ApprovalStatus == ApproveType.Accepted) ? ModerationStatus.Accepted : ModerationStatus.Rejected;
                }
            }

            return status;
        }

        private bool IsContestLocked(Contest contest)
        {
            bool result = false;

            if (contest == null)
            {
                result = false;
            }
            else
            {
                var now = DateTime.UtcNow;
                result = contest.StartDateTimeUTC.AddMinutes(-Constants.ContestLockBeforeStartInMinutes) <= now
                            && now < contest.EndDateTimeUTC;
            }

            return result;
        }

        private async Task<EntityInLockedContestStatus> IsEntityUsedInLockedContestsAsync(MainDbContext dbContext, long userId, Expression<Func<Contest, bool>> predicate)
        {
            var result = EntityInLockedContestStatus.Undefined;

            var contests = await dbContext.Contests.Where(predicate).ToListAsync();

            contests = contests.Where(c => IsContestLocked(c)).ToList();
            if (contests != null && contests.Count > 0)
            {
                if (contests.Any(c => c.CreatorId == userId))
                {
                    result = EntityInLockedContestStatus.LockedByOwnContest;
                }
                else
                {
                    result = EntityInLockedContestStatus.LockedByOthersContests;
                }
            }
            else
            {
                result = EntityInLockedContestStatus.NotLocked;
            }

            return result;
        }

        private async Task<bool> UpdateLinkedEntitiesAsync<TEntityForm, TEntity, TIdentity>(MainDbContext dbContext, DbSet<TEntity> dbSet,
            List<TEntityForm> entitiesFromForm, Expression<Func<TEntity, bool>> predicateForDbSearch,
            Func<TEntity, TIdentity> predicateForIdentity, Func<TIdentity, TIdentity, bool> predicateForIdentityCompare,
            Func<TEntityForm, TEntity> predicateForFormParse, Func<TEntity, TEntityForm, TEntity> predicateBeforeUpdate,
            bool needToSave = false)
            where TEntity : class
            where TEntityForm : class
        {
            var entities = await dbSet.Where(predicateForDbSearch).ToListAsync();
            var entitiesExamined = new Dictionary<TIdentity, bool>();
            foreach (var entity in entities)
            {
                entitiesExamined.Add(predicateForIdentity(entity), false);
            }

            for (int i = 0; i < entitiesFromForm.Count; i++)
            {
                var entity = predicateForFormParse(entitiesFromForm[i]);
                var loadedEntity = entities.FirstOrDefault(e => predicateForIdentityCompare(predicateForIdentity(e), predicateForIdentity(entity)));
                if (loadedEntity == null)
                {
                    await dbSet.AddAsync(entity);
                }
                else
                {
                    entitiesExamined[predicateForIdentity(entity)] = true;
                    loadedEntity = predicateBeforeUpdate(loadedEntity, entitiesFromForm[i]);
                    dbSet.Update(loadedEntity);
                }
            }

            foreach (var item in entitiesExamined)
            {
                if (!item.Value)
                {
                    var loadedEntity = entities.FirstOrDefault(e => predicateForIdentityCompare(predicateForIdentity(e), item.Key));
                    if (loadedEntity != null)
                    {
                        dbSet.Remove(loadedEntity);
                    }
                }
            }

            return needToSave ? await dbContext.SecureSaveAsync() : true; // НЕ ПРИНИМАТЬ предложение IDE упростить данное выражение
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

            return needToSave ? await dbContext.SecureSaveAsync() : true; // НЕ ПРИНИМАТЬ предложение IDE упростить данное выражение
        }
    }
}
