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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public ContestsController(MainDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("get-available-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableContests(string culture)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var participatingContests = await _dbContext.ContestsParticipants
                .Where(cp => cp.ParticipantId == currentUser.Id)
                .Select(cp => cp.Contest)
                .ToListAsync();
            var moderatedContests = await _dbContext.ContestsLocalModerators
                .Where(clm => clm.LocalModeratorId == currentUser.Id)
                .Select(cp => cp.Contest)
                .ToListAsync();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC > DateTime.UtcNow
                                                                && c.IsPublic
                                                                && participatingContests.All(pc => pc.Id != c.Id)
                                                                && moderatedContests.All(mc => mc.Id != c.Id)
                )
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount));
            }

            return Json(publishedContests);
        }

        [HttpGet("get-running-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetRunningContests(string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC <= DateTime.UtcNow
                                                                && c.EndDateTimeUTC > DateTime.UtcNow
                                                                && c.RulesSet.PublicMonitor)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount));
            }

            return Json(publishedContests);
        }

        [HttpGet("get-participating-contests/{culture}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetParticipatingContests(string culture)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var userContests = await _dbContext.ContestsParticipants.Where(cp => cp.ParticipantId == currentUser.Id)
                .ToListAsync();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.EndDateTimeUTC >= DateTime.UtcNow
                                                                && userContests.Any(uc => uc.ContestId == c.Id))
                .OrderBy(c => c.StartDateTimeUTC)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount));
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
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Такой локализации под данный контест не существует"}
                    });
                }

                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contest.Id);
                var publishedContest = PublishedContest.GetFromModel(contest, localizer, participantsCount);
                return Json(publishedContest);
            }

            return Json(new
            {
                status = false,
                errors = new List<string> {"Контеста с таким идентификатором не существует"}
            });
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedContest(long id)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(p => p.Id == id);
            if (contest != null)
            {
                var problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contest.Id).ToListAsync();
                var constructedContest = ConstructedContest.GetFromModel(contest, problems);
                return Json(constructedContest);
            }

            return Json(new
            {
                status = false,
                errors = new List<string> {"Контеста с таким идентификатором не существует"}
            });
        }

        [HttpGet("get-user-created-contests/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCreatedContests(long id, string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.CreatorId == id).ToListAsync();
            var publishedContests = contests.ConvertAll(async c =>
            {
                var localizer = c.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                int participantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == c.Id);
                var pc = PublishedContest.GetFromModel(c, localizer, participantsCount);
                return pc;
            });
            return Json(publishedContests.Select(pc => pc.Result));
        }

        [HttpPost("add-contest")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddContest([FromForm] ContestForm contestForm)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(contestForm.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int) contestForm.Image.Length);
                }

                Contest contest = new Contest
                {
                    CreatorId = contestForm.CreatorUserId,
                    StartDateTimeUTC = contestForm.StartDateTimeUTC,
                    DurationInMinutes = contestForm.DurationInMinutes,
                    AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable,
                    Image = Convert.ToBase64String(imageData),
                    IsPublic = contestForm.IsPublic,
                    ContestLocalizers = new List<ContestLocalizer>(),
                    RulesSetId = contestForm.RulesSetId
                };
                /*
                TODO: нормальная проверка на лимиты
                var user = await HttpContext.GetCurrentUser(_userManager);
                if (user.IsLimitedInContests)
                {
                    if (await _dbContext.Contests.AnyAsync(c => c.CreatorId == user.Id))
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Превышено ограничение недоверенного пользователя по созданию контестов" }
                        });
                    }
                    contest.ApprovalStatus = ApproveType.NotModeratedYet;
                }
                else
                {
                    contest.ApprovalStatus = ApproveType.Accepted;
                }*/
                contest.ApprovalStatus = ApproveType.Accepted;
                await _dbContext.Contests.AddAsync(contest);
                await _dbContext.SaveChangesAsync();
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-contest/{id}")]
        public async Task<IActionResult> EditContest([FromForm] ContestForm contestForm, long id)
        {
            if (contestForm.Id == null || id <= 0 || id != contestForm.Id)
            {
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
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка изменить несуществующий контест"}
                    });
                }
                else
                {
                    if (HttpContext.GetCurrentUser().GetAwaiter().GetResult().Id != contest.CreatorId)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Попытка изменить не свой контест"}
                        });
                    }

                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(contestForm.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int) contestForm.Image.Length);
                    }

                    contest.CreatorId = contestForm.CreatorUserId;
                    contest.Image = Convert.ToBase64String(imageData);
                    contest.StartDateTimeUTC = contestForm.StartDateTimeUTC;
                    contest.DurationInMinutes = contestForm.DurationInMinutes;
                    contest.AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable;
                    contest.IsPublic = contestForm.IsPublic;
                    contest.RulesSetId = contestForm.RulesSetId;
                    _dbContext.Contests.Update(contest);
                    for (int i = 0; i < contestForm.Localizers.Count; i++)
                    {
                        var localizer = new ContestLocalizer
                        {
                            Culture = contestForm.Localizers[i].Culture,
                            Description = contestForm.Localizers[i].Description,
                            Name = contestForm.Localizers[i].Name,
                            ContestId = contest.Id
                        };
                        var loadedLocalizer =
                            await _dbContext.ContestsLocalizers.FirstOrDefaultAsync(pl =>
                                pl.Culture == localizer.Culture && pl.ContestId == id);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.ContestsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            loadedLocalizer.Description = localizer.Description;
                            loadedLocalizer.Name = localizer.Name;
                            _dbContext.ContestsLocalizers.Update(loadedLocalizer);
                        }
                    }

                    for (int i = 0; i < contestForm.Problems.Count; i++)
                    {
                        var contestProblem = new ContestProblem
                        {
                            ContestId = contest.Id,
                            ProblemId = contestForm.Problems[i].ProblemId,
                            Letter = contestForm.Problems[i].Letter
                        };
                        var loadedContestProblem = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp =>
                            cp.ProblemId == contestProblem.ProblemId && cp.ContestId == contestProblem.ContestId);
                        if (loadedContestProblem == null)
                        {
                            await _dbContext.ContestsProblems.AddAsync(contestProblem);
                        }
                        else
                        {
                            loadedContestProblem.Letter = contestForm.Problems[i].Letter;
                            _dbContext.ContestsProblems.Update(loadedContestProblem);
                        }
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpDelete("delete-contest/{id}")]
        public async Task<IActionResult> DeleteContest(long id)
        {
            Contest loadedContest = await _dbContext.Contests.FindAsync(id);
            if (loadedContest == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий контест"}
                });
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedContest.CreatorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой контест или без модераторских прав"}
                });
            }

            _dbContext.Contests.Remove(loadedContest);
            await _dbContext.SaveChangesAsync();
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
            if (ModelState.IsValid)
            {
                var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
                if (contest == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка добавить участника в несуществующий контест"}
                    });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participantForm.UserId);
                if (user == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка добавить несуществующего пользователя в контест"}
                    });
                }

                if (await _dbContext.ContestsParticipants.AnyAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == participantForm.UserId))
                {
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
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка добавить участника в несуществующий контест"}
                });
            }

            var contestParticipant =
                await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == userId);
            if (contestParticipant == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Такого участника нет в контесте"}
                });
            }

            _dbContext.ContestsParticipants.Remove(contestParticipant);
            await _dbContext.SaveChangesAsync();
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
            var contestParticipants = await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();
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
            var contestParticipants = await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();
            var solutions = await _dbContext.Solutions.Where(s => s.ContestId == contestId).ToListAsync();
            var monitorEntries = new List<MonitorEntry>();
            var problems = contest.ContestProblems.OrderBy(cp => cp.Letter).ToList();
            foreach (var cp in contestParticipants)
            {
                var participantSolutions = solutions.Where(s => s.ParticipantId == cp.ParticipantId);
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
                    var participantProblemSolutions = participantSolutions.Where(ps => ps.ProblemId == problem.ProblemId)
                                                                            .OrderBy(ps => ps.SubmitTimeUTC)
                                                                            .ToList();
                    var problemTriesEntry = new ProblemTriesEntry
                    {
                        ContestId = contestId,
                        UserId = cp.ParticipantId,
                        ProblemId = problem.ProblemId,
                        Letter = problem.Letter,
                        TriesCount = participantProblemSolutions.Count,
                        LastTryMinutesAfterStart = (short)(participantProblemSolutions.Last().SubmitTimeUTC - contest.StartDateTimeUTC).TotalMinutes,
                        Solved = participantProblemSolutions.Any(pps => pps.Verdict == VerdictType.Accepted || pps.Verdict == VerdictType.PartialSolution)
                    };
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
                    monitorEntry.ProblemTries.Add(problemTriesEntry);
                }
                monitorEntries.Add(monitorEntry);
            }
            if (contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
            {
                monitorEntries = monitorEntries.OrderByDescending(me => me.ProblemsSolvedCount).ThenBy(me => me.Result).ToList();
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
            return Json((await _dbContext.Solutions
                    .Where(s => s.ContestId == contestId)
                    .Where(s => s.ParticipantId == userId)
                    .ToListAsync()).ConvertAll(s => s.ResponseStructure)
            );
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetContestsRequests()
        {
            var contests = await _dbContext.Contests.Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet).ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems);
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
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems);
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
                var cr = ConstructedContest.GetFromModel(c, c.ContestProblems);
                return cr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectContest([FromBody] ContestRequestForm contestRequestForm, long id)
        {
            if (contestRequestForm.ContestId != id || id < 0)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }

            if (ModelState.IsValid)
            {
                var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                if (contest == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка модерировать несуществующий пост" }
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
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }

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