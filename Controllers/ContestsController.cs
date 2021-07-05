using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ContestSystem.Services;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ContestsController> _logger;
        private readonly FileStorageService _storage;

        public ContestsController(MainDbContext dbContext, UserManager<User> userManager, ILogger<ContestsController> logger, FileStorageService storage)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _storage = storage;
        }

        [HttpGet("get-available-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableContests(string culture)
        {
            DateTime now = DateTime.UtcNow;
            var currentUser = await HttpContext.GetCurrentUser();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC > now
                                                                && c.IsPublic
                                                                && c.ContestParticipants.All(cp =>
                                                                    cp.ParticipantId != currentUser.Id)
                                                                && c.ContestLocalModerators.All(cp =>
                                                                    cp.LocalModeratorId != currentUser.Id))
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount, _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("get-running-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetRunningContests(string culture)
        {
            DateTime now = DateTime.UtcNow;
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC <= now
                                                                && c.StartDateTimeUTC.AddMinutes(c.DurationInMinutes) > now
                                                                && c.RulesSet.PublicMonitor)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount, _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("get-participating-contests/{culture}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetParticipatingContests(string culture)
        {
            DateTime now = DateTime.UtcNow;
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC >=
                                                                now.AddMinutes(-c.DurationInMinutes)
                                                                && c.ContestParticipants.Any(cp =>
                                                                    cp.ParticipantId == currentUser.Id))
                .OrderBy(c => c.StartDateTimeUTC)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount, _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedContest(long id, string culture)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
            if (contest != null)
            {
                var localizer = contest.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return NotFound("Такой локализации под контест не существует");
                }

                int participantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contest.Id);
                var publishedContest = PublishedContest.GetFromModel(contest, localizer, participantsCount, _storage.GetImageInBase64(contest.ImagePath));
                return Json(publishedContest);
            }

            return NotFound("Такого контеста не существует");
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedContest(long id)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(p => p.Id == id);
            if (contest != null)
            {
                var problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contest.Id).ToListAsync();
                var constructedContest = ConstructedContest.GetFromModel(contest, problems, _storage.GetImageInBase64(contest.ImagePath));
                return Json(constructedContest);
            }

            return NotFound("Такого контеста не существует");
        }

        [HttpGet("get-user-created-contests/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCreatedContests(long id, string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.CreatorId == id).ToListAsync();
            var publishedContests = contests.ConvertAll(c =>
            {
                var localizer = c.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                int participantsCount = c.ContestParticipants.Count(cp => cp.ContestId == c.Id);
                var pc = PublishedContest.GetFromModel(c, localizer, participantsCount, _storage.GetImageInBase64(c.ImagePath));
                return pc;
            });
            return Json(publishedContests);
        }

        [HttpPost("add-contest")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddContest([FromForm] ContestForm contestForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (ModelState.IsValid)
            {
                Contest contest = new Contest
                {
                    CreatorId = contestForm.CreatorUserId,
                    StartDateTimeUTC = contestForm.StartDateTimeUTC,
                    DurationInMinutes = contestForm.DurationInMinutes,
                    AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable,
                    IsPublic = contestForm.IsPublic,
                    ContestLocalizers = new List<ContestLocalizer>(),
                    RulesSetId = contestForm.RulesSetId
                };

                if (currentUser.Id != contestForm.CreatorUserId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator("Contest", currentUser.Id,
                        contestForm.CreatorUserId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Id автора в форме отличается от Id текущего пользователя"}
                    });
                }

                if (currentUser.IsLimitedInContests)
                {
                    if (await _dbContext.Contests.CountAsync(c => c.CreatorId == currentUser.Id && c.ApprovalStatus == ApproveType.NotModeratedYet) == 1)
                    {
                        _logger.LogCreationFailedBecauseOfLimits("Contest", currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string>
                                {"Превышено ограничение недоверенного пользователя по созданию контестов"}
                        });
                    }

                    contest.ApprovalStatus = ApproveType.NotModeratedYet;
                }
                else
                {
                    contest.ApprovalStatus = ApproveType.Accepted;
                }

                await _dbContext.Contests.AddAsync(contest);
                await _dbContext.SaveChangesAsync();
                contest.ImagePath = await _storage.SaveContestImageAsync(contest.Id, contestForm.Image);
                _dbContext.Contests.Update(contest);

                var localModerator = new ContestLocalModerator
                {
                    ContestId = contest.Id,
                    Alias = contest.Creator.FullName, // TODO: надо получать алиас из формы
                    LocalModeratorId = contest.CreatorId.GetValueOrDefault()
                };
                await _dbContext.ContestsLocalModerators.AddAsync(localModerator);
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < contestForm.Localizers.Count; i++)
                {
                    var localizer = new ContestLocalizer
                    {
                        Culture = contestForm.Localizers[i].Culture,
                        Description = contestForm.Localizers[i].Description,
                        Name = contestForm.Localizers[i].Name,
                        ContestId = contest.Id
                    };
                    contest.ContestLocalizers.Add(localizer);
                }

                for (int i = 0; i < contestForm.Problems.Count; i++)
                {
                    var contestProblems = new ContestProblem
                    {
                        ContestId = contest.Id,
                        ProblemId = contestForm.Problems[i].ProblemId,
                        Letter = contestForm.Problems[i].Letter
                    };
                    await _dbContext.ContestsProblems.AddAsync(contestProblems);
                }

                await _dbContext.SaveChangesAsync();
                if (contest.ApprovalStatus == ApproveType.Accepted)
                {
                    _logger.LogCreationSuccessfulWithAutoAccept("Contest", contest.Id, currentUser.Id);
                }
                else
                {
                    _logger.LogCreationSuccessful("Contest", contest.Id, currentUser.Id);
                }

                return Json(new
                {
                    status = true,
                    data = contest.Id,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList()
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-contest/{id}")]
        public async Task<IActionResult> EditContest([FromForm] ContestForm contestForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (contestForm.Id == null || id <= 0 || id != contestForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId("Contest", contestForm.Id, id, currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                Contest contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                if (contest == null)
                {
                    _logger.LogEditingOfNonExistentEntity("Contest", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка изменить несуществующий контест"}
                    });
                }
                else
                {
                    bool needToRemoderate = (contestForm.StartDateTimeUTC != contest.StartDateTimeUTC);
                    if (!await _dbContext.ContestsLocalModerators.AnyAsync(clm =>
                        clm.ContestId == id && clm.LocalModeratorId == currentUser.Id))
                    {
                        _logger.LogEditingByNotAppropriateUser("Contest", id, currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Попытка изменить не свой контест"}
                        });
                    }

                    if (contestForm.Image != null)
                    {
                        contest.ImagePath = await _storage.SaveContestImageAsync(id, contestForm.Image);
                    }
              
                    contest.StartDateTimeUTC = contestForm.StartDateTimeUTC;
                    contest.DurationInMinutes = contestForm.DurationInMinutes;
                    contest.AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable;
                    contest.IsPublic = contestForm.IsPublic;
                    contest.RulesSetId = contestForm.RulesSetId;
                    if (needToRemoderate || contest.ApprovalStatus == ApproveType.Rejected)
                    {
                        contest.ApprovalStatus = ApproveType.NotModeratedYet;
                        contest.ApprovingModeratorId = null;
                    }

                    _dbContext.Contests.Update(contest);

                    var localizers = await _dbContext.ContestsLocalizers.Where(l => l.ContestId == id).ToListAsync();
                    var localizersExamined = new Dictionary<long, bool>();
                    foreach (var l in localizers)
                    {
                        localizersExamined.Add(l.Id, false);
                    }

                    for (int i = 0; i < contestForm.Localizers.Count; i++)
                    {
                        var localizer = new ContestLocalizer
                        {
                            Culture = contestForm.Localizers[i].Culture,
                            Description = contestForm.Localizers[i].Description,
                            Name = contestForm.Localizers[i].Name,
                            ContestId = contest.Id
                        };
                        var loadedLocalizer = localizers.FirstOrDefault(l => l.Culture == localizer.Culture);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.ContestsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            localizersExamined[loadedLocalizer.Id] = true;
                            loadedLocalizer.Description = localizer.Description;
                            loadedLocalizer.Name = localizer.Name;
                            _dbContext.ContestsLocalizers.Update(loadedLocalizer);
                        }
                    }

                    foreach (var item in localizersExamined)
                    {
                        if (!item.Value)
                        {
                            var loadedLocalizer = localizers.FirstOrDefault(l => l.Id == item.Key);
                            _dbContext.ContestsLocalizers.Remove(loadedLocalizer);
                        }
                    }

                    var problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == id).ToListAsync();
                    var problemsExamined = new Dictionary<long, bool>();
                    foreach (var p in problems)
                    {
                        problemsExamined.Add(p.Id, false);
                    }

                    for (int i = 0; i < contestForm.Problems.Count; i++)
                    {
                        var contestProblem = new ContestProblem
                        {
                            ContestId = contest.Id,
                            ProblemId = contestForm.Problems[i].ProblemId,
                            Letter = contestForm.Problems[i].Letter
                        };
                        var loadedContestProblem =
                            problems.FirstOrDefault(cp => cp.ProblemId == contestProblem.ProblemId);
                        if (loadedContestProblem == null)
                        {
                            await _dbContext.ContestsProblems.AddAsync(contestProblem);
                        }
                        else
                        {
                            problemsExamined[loadedContestProblem.Id] = true;
                            loadedContestProblem.Letter = contestForm.Problems[i].Letter;
                            _dbContext.ContestsProblems.Update(loadedContestProblem);
                        }
                    }

                    foreach (var item in problemsExamined)
                    {
                        if (!item.Value)
                        {
                            var loadedProblem = problems.FirstOrDefault(p => p.Id == item.Key);
                            _dbContext.ContestsProblems.Remove(loadedProblem);
                        }
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Contest", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

                    _logger.LogEditingSuccessful("Contest", id, currentUser.Id);
                    return Json(new
                    {
                        status = true,
                        errors = new List<string>()
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList()
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-contest/{id}")]
        public async Task<IActionResult> DeleteContest(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            Contest loadedContest = await _dbContext.Contests.FindAsync(id);
            if (loadedContest == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy("Contest", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий контест"}
                });
            }

            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedContest.CreatorId && !currentUser.Roles.Contains(moderatorRole))
            {
                _logger.LogDeletingByNotAppropriateUser("Contest", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой контест или без модераторских прав"}
                });
            }

            _storage.DeleteFileAsync(loadedContest.ImagePath);
            _dbContext.Contests.Remove(loadedContest);
            await _dbContext.SaveChangesAsync();
            _logger.LogDeletingSuccessful("Contest", id, currentUser.Id);
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }

        [HttpPost("{contestId}/add-participant")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddParticipant(long contestId, [FromBody] ParticipantForm participantForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (contestId <= 0 || contestId != participantForm.ContestId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} записаться на участие в соревновании с идентификатором {contestId}, когда в переданной форме указано соревнование с идентификатором {participantForm.ContestId}");
                return Json(new
                {
                    success = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
                if (contest == null)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} принять участие в несуществующем соревновании с идентификатором {contestId}");
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка добавить участника в несуществующий контест"}
                    });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participantForm.UserId);
                if (user == null)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя {currentUser.Id} добавить несуществующего пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}");
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка добавить несуществующего пользователя в контест"}
                    });
                }

                if (await _dbContext.ContestsParticipants.AnyAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == participantForm.UserId))
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя {currentUser.Id} добавить пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}, когда данный пользователь уже является участником данного соревнования");
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Такой участник в контесте уже есть"}
                    });
                }

                var contestParticipant = new ContestParticipant
                {
                    ParticipantId = participantForm.UserId,
                    ContestId = participantForm.ContestId,
                    Alias = participantForm.Alias,
                    Result = 0,
                    ConfirmedByParticipant = true, // TODO: сделать нормальные проверки на инвайты и прочую чепухню
                    ConfirmedByLocalModerator = true,
                    ConfirmingLocalModeratorId = contest.CreatorId
                };
                await _dbContext.ContestsParticipants.AddAsync(contestParticipant);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation(
                    $"В соревнование с идентификатором {contest.Id} успешно добавлен участник с идентификатором {participantForm.UserId} пользователем с идентификатором {currentUser.Id}");
                return Json(new
                {
                    status = true,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList()
            });
        }

        [HttpDelete("{contestId}/delete-participant/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> DeleteParticipant(long contestId, long userId)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников несуществующего соревнования с идентификатором {contestId} пользователя с идентификатором {userId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить участника из несуществующего контеста"}
                });
            }

            var contestParticipant =
                await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == userId);
            if (contestParticipant == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников соревнования с идентификатором {contestId} несуществующего участника с идентификатором {userId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Такого участника нет в контесте"}
                });
            }

            _dbContext.ContestsParticipants.Remove(contestParticipant);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation(
                $"Пользователем с идентификатором {currentUser.Id} из списка участников соревнования с идентификатором {contestId} успешно удалён пользователь с идентификатором {userId}");
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }

        [HttpGet("{contestId}/get-participants")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetParticipants(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound("Такого контеста не существует");
            }

            var contestParticipants =
                await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();
            var participants = contestParticipants.ConvertAll(cp =>
            {
                var p = ParticipantExternalModel.GetFromModel(cp);
                return p;
            });
            return Json(participants);
        }

        [HttpGet("{contestId}/get-monitor")]
        public async Task<IActionResult> GetMonitor(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound("Такого контеста не существует");
            }

            DateTime now = DateTime.UtcNow;
            var contestParticipants =
                await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();
            var solutions = await _dbContext.Solutions.Where(s => s.ContestId == contestId
                                                                  && (s.SubmitTimeUTC <
                                                                      contest.EndDateTimeUTC.AddMinutes(-contest
                                                                          .RulesSet
                                                                          .MonitorFreezeTimeBeforeFinishInMinutes)
                                                                      || contest.EndDateTimeUTC <= now))
                .ToListAsync();
            var monitorEntries = new List<MonitorEntry>();
            var problems = contest.ContestProblems.OrderBy(cp => cp.Letter).ToList();
            foreach (var cp in contestParticipants)
            {
                var participantSolutions = solutions.Where(s => s.ParticipantId == cp.ParticipantId).ToList();
                var monitorEntry = new MonitorEntry
                {
                    ContestId = contestId,
                    UserId = cp.ParticipantId,
                    Alias = cp.Alias,
                    Position = 0,
                    Result = cp.Result,
                    ProblemsSolvedCount = 0,
                    ProblemTries = new List<ProblemTriesEntry>()
                };
                foreach (var problem in problems)
                {
                    var participantProblemSolutions = participantSolutions
                        .Where(ps => ps.ProblemId == problem.ProblemId)
                        .OrderBy(ps => ps.SubmitTimeUTC)
                        .ToList();
                    var problemTriesEntry = new ProblemTriesEntry
                    {
                        ContestId = contestId,
                        UserId = cp.ParticipantId,
                        ProblemId = problem.ProblemId,
                        Letter = problem.Letter,
                        TriesCount = participantProblemSolutions.Count,
                    };
                    if (participantProblemSolutions == null || participantProblemSolutions.Count == 0)
                    {
                        problemTriesEntry.LastTryMinutesAfterStart = 0;
                        problemTriesEntry.Solved = false;
                        problemTriesEntry.GotPoints = 0;
                    }
                    else
                    {
                        problemTriesEntry.LastTryMinutesAfterStart = (short)(participantProblemSolutions.Last().SubmitTimeUTC - contest.StartDateTimeUTC).TotalMinutes;
                        problemTriesEntry.Solved = participantProblemSolutions.Any(pps => pps.Verdict == VerdictType.Accepted || pps.Verdict == VerdictType.PartialSolution);

                        if (problemTriesEntry.Solved)
                        {
                            monitorEntry.ProblemsSolvedCount++;
                        }

                        if (contest.RulesSet.PointsForBestSolution)
                        {
                            problemTriesEntry.GotPoints = participantProblemSolutions.Max(pps => pps.Points);
                        }
                        else
                        {
                            problemTriesEntry.GotPoints = participantProblemSolutions.Last().Points;
                        }
                    }

                    monitorEntry.ProblemTries.Add(problemTriesEntry);
                }

                monitorEntries.Add(monitorEntry);
            }

            if (contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
            {
                monitorEntries = monitorEntries.OrderByDescending(me => me.ProblemsSolvedCount).ThenBy(me => me.Result)
                    .ToList();
            }
            else
            {
                monitorEntries = monitorEntries.OrderByDescending(me => me.Result).ToList();
            }

            for (int i = 0; i < monitorEntries.Count; i++)
            {
                monitorEntries[i].Position = i + 1;
            }

            return Json(monitorEntries);
        }

        [HttpGet("{contestId}/get-solutions/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserSolutions(long contestId, long userId)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках несуществующего соревнования с идентификатором {contestId}");
                return BadRequest("Такого соревнования не существует");
            }
            if (currentUser.Id != userId && !await _dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == contestId && clm.LocalModeratorId == currentUser.Id))
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках соревнования с идентификатором {contestId} при отсутствии прав на их получение");
                return BadRequest("Данный пользователь не имеет доступа к этим данным");
            }
            if (!await _dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == contestId && cp.ParticipantId == userId))
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках соревнования с идентификатором {contestId}, когда среди участников такого пользователя нет");
                return NotFound("Такого участника в соревновании нет");
            }
            var solutions = await _dbContext.Solutions.Where(s => s.ContestId == contestId && s.ParticipantId == userId).ToListAsync();
            return Json(solutions.ConvertAll(s => ConstructedSolution.GetFromModel(s, s.Contest.ContestProblems, _storage.GetImageInBase64(s.Contest.ImagePath))));
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetContestsRequests()
        {
            var contests = await _dbContext.Contests.Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet)
                .ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedContests()
        {
            var contests = await _dbContext.Contests.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedContests()
        {
            var contests = await _dbContext.Contests.Where(p => p.ApprovalStatus == ApproveType.Rejected).ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectContest([FromBody] ContestRequestForm contestRequestForm,
            long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (contestRequestForm.ContestId != id || id < 0)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId("Contest", contestRequestForm.ContestId, id,
                    currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                if (contest == null)
                {
                    _logger.LogModeratingOfNonExistentEntity("Contest", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка модерировать несуществующий контест"}
                    });
                }
                else
                {
                    contest.ApprovalStatus = contestRequestForm.ApprovalStatus;
                    contest.ApprovingModeratorId = contestRequestForm.ApprovingModeratorId;
                    contest.ModerationMessage = contestRequestForm.ModerationMessage;
                    _dbContext.Contests.Update(contest);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Contest", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

                    _logger.LogModeratingSuccessful("Contest", id, currentUser.Id, contestRequestForm.ApprovalStatus);
                    return Json(new
                    {
                        status = true,
                        errors = new List<string>()
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}