using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ContestSystem.Services;
using ContestSystem.Models.Misc;
using Microsoft.AspNetCore.Identity;
using ContestSystem.Models.Dictionaries;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<ContestsController> _logger;
        private readonly FileStorageService _storage;
        private readonly WorkspaceManagerService _workspace;
        private readonly UserManager<User> _userManager;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public ContestsController(MainDbContext dbContext, ILogger<ContestsController> logger, FileStorageService storage, WorkspaceManagerService workspace, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _workspace = workspace;
            _userManager = userManager;
            _errorCodes = Constants.ErrorCodes[_entityName];
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
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount,
                    _storage.GetImageInBase64(contests[i].ImagePath)));
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
                                                                && c.StartDateTimeUTC.AddMinutes(c.DurationInMinutes) >
                                                                now
                                                                && c.RulesSet.PublicMonitor)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount,
                    _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("get-participating-contests/{culture}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetParticipatingContests(string culture)
        {
            DateTime now = DateTime.UtcNow;
            var currentUser = await HttpContext.GetCurrentUser();
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
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(PublishedContest.GetFromModel(contests[i], localizers[i], participantsCount,
                    _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetPublishedContest(long id, string culture)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
            if (contest != null)
            {
                var localizer = contest.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return NotFound(_errorCodes[Constants.EntityLocalizerDoesntExistErrorName]);
                }

                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contest.Id);
                var publishedContest = PublishedContest.GetFromModel(contest, localizer, participantsCount,
                    _storage.GetImageInBase64(contest.ImagePath));
                return Json(publishedContest);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedContest(long id)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(p => p.Id == id);
            if (contest != null)
            {
                var problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contest.Id).ToListAsync();
                var constructedContest =
                    ConstructedContest.GetFromModel(contest, problems, _storage.GetImageInBase64(contest.ImagePath));
                return Json(constructedContest);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
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
                var pc = PublishedContest.GetFromModel(c, localizer, participantsCount,
                    _storage.GetImageInBase64(c.ImagePath));
                return pc;
            });
            return Json(publishedContests);
        }

        [HttpPost("add-contest")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddContest([FromForm] ContestForm contestForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            ResponseObject<long> response = new ResponseObject<long>();
            if (ModelState.IsValid)
            {
                if (currentUser.Id != contestForm.CreatorUserId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id, contestForm.CreatorUserId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
                }
                else
                {
                    CreationStatusData statusData = await _workspace.CreateContestAsync(_dbContext, contestForm, currentUser.IsLimitedInContests);
                    _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForCreation(statusData.Status, _entityName, statusData.Id);
                }
            }
            else
            {
                response = ResponseObject<long>.Fail(ModelState, _entityName);
            }
            return Json(response);
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-contest/{id}")]
        public async Task<IActionResult> EditContest([FromForm] ContestForm contestForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            ResponseObject<long> response = new ResponseObject<long>();
            if (contestForm.Id == null || id <= 0 || id != contestForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId(_entityName, contestForm.Id, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Contest contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                    if (contest == null)
                    {
                        _logger.LogEditingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (!contest.ContestLocalModerators.Any(clm => clm.LocalModeratorId == currentUser.Id))
                        {
                            _logger.LogEditingByNotAppropriateUser(_entityName, id, currentUser.Id);
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                        }
                        else
                        {
                            EditionStatus status = await _workspace.EditContestAsync(_dbContext, contestForm, currentUser.Id, contest);
                            _logger.LogEditionStatus(status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForEdition(status, _entityName, id);
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, _entityName);
                }
            }
            return Json(response);
        }

        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-contest/{id}")]
        public async Task<IActionResult> DeleteContest(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            ResponseObject<long> response = new ResponseObject<long>();
            Contest loadedContest = await _dbContext.Contests.FindAsync(id);
            if (loadedContest == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(_entityName, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                if (currentUser.Id != loadedContest.CreatorId && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
                {
                    _logger.LogDeletingByNotAppropriateUser(_entityName, id, currentUser.Id);
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    DeletionStatus status = await _workspace.DeleteContestAsync(_dbContext, loadedContest, currentUser.Id);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                }
            }
            return Json(response);
        }

        [HttpPost("{contestId}/add-participant")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddParticipant(long contestId, [FromBody] ParticipantForm participantForm)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser();

            if (contestId <= 0 || contestId != participantForm.ContestId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} добавить участника с идентификатором {participantForm.UserId} с идентификатором {contestId}, когда в переданной форме указано соревнование с идентификатором {participantForm.ContestId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
                    if (contest == null)
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} добавить участника с идентификатором {participantForm.UserId} в несуществующее соревнование с идентификатором {contestId}");
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participantForm.UserId);
                        if (user == null)
                        {
                            _logger.LogWarning(
                                $"Попытка от пользователя {currentUser.Id} добавить несуществующего пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}");
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                        }
                        else
                        {
                            if (await _dbContext.ContestsParticipants.AnyAsync(cp =>
                            cp.ContestId == contestId && cp.ParticipantId == participantForm.UserId))
                            {
                                _logger.LogWarning(
                                    $"Попытка от пользователя {currentUser.Id} добавить пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}, когда данный пользователь уже является участником данного соревнования");
                                response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserAlreadyInContestErrorName]);
                            }
                            else
                            {
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
                                response = ResponseObject<long>.Success(contestId);
                            }
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, Constants.UserEntityName);
                }
            }
            return Json(response);
        }

        [HttpDelete("{contestId}/delete-participant/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> DeleteParticipant(long contestId, long userId)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser();
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников несуществующего соревнования с идентификатором {contestId} пользователя с идентификатором {userId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                var contestParticipant =
                await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == userId);
                if (contestParticipant == null)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников соревнования с идентификатором {contestId} несуществующего участника с идентификатором {userId}");
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                }
                else
                {
                    _dbContext.ContestsParticipants.Remove(contestParticipant);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation(
                        $"Пользователем с идентификатором {currentUser.Id} из списка участников соревнования с идентификатором {contestId} успешно удалён пользователь с идентификатором {userId}");
                    response = ResponseObject<long>.Success(userId);
                }
            }
            return Json(response);
        }

        [HttpGet("{contestId}/get-participants")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetParticipants(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var contestParticipants =
                await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();
            var participants = contestParticipants.ConvertAll(ParticipantExternalModel.GetFromModel);
            return Json(participants);
        }

        [HttpGet("{contestId}/get-all-solutions")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetAllSolutions(long contestId)
        {
            return null;
        }

        [HttpGet("{contestId}/get-monitor")]
        public async Task<IActionResult> GetMonitor(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
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
                        problemTriesEntry.LastTryMinutesAfterStart =
                            (short)(participantProblemSolutions.Last().SubmitTimeUTC - contest.StartDateTimeUTC)
                            .TotalMinutes;
                        problemTriesEntry.Solved = participantProblemSolutions.Any(pps =>
                            pps.Verdict == VerdictType.Accepted || pps.Verdict == VerdictType.PartialSolution);

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
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetUserSolutions(long contestId, long userId)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            if (currentUser.Id != userId && !await _dbContext.ContestsLocalModerators.AnyAsync(clm =>
                clm.ContestId == contestId && clm.LocalModeratorId == currentUser.Id))
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках соревнования с идентификатором {contestId} при отсутствии прав на их получение");
                return BadRequest(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
            }

            if (!await _dbContext.ContestsParticipants.AnyAsync(cp =>
                cp.ContestId == contestId && cp.ParticipantId == userId))
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить отправки пользователя с идентификатором {userId} в рамках соревнования с идентификатором {contestId}, когда среди участников такого пользователя нет");
                return NotFound(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
            }

            var solutions = await _dbContext.Solutions.Where(s => s.ContestId == contestId && s.ParticipantId == userId)
                .ToListAsync();
            return Json(solutions.ConvertAll(s =>
                ConstructedSolution.GetFromModel(s, s.Contest.ContestProblems,
                    _storage.GetImageInBase64(s.Contest.ImagePath))));
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetContestsRequests()
        {
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.NotModeratedYet)
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
            var currentUser = await HttpContext.GetCurrentUser();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.ApprovingModeratorId.GetValueOrDefault(-1) ==
                                                                currentUser.Id)
                .ToListAsync();
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
            var currentUser = await HttpContext.GetCurrentUser();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Rejected
                                                                && c.ApprovingModeratorId.GetValueOrDefault(-1) ==
                                                                currentUser.Id)
                .ToListAsync();
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
            ResponseObject<long> response = new ResponseObject<long>();
            if (contestRequestForm.ContestId != id || id < 0)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId(_entityName, contestRequestForm.ContestId, id,
                    currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                    if (contest == null)
                    {
                        _logger.LogModeratingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (contest.ApprovingModeratorId.GetValueOrDefault(-1) != currentUser.Id &&
                            contest.ApprovalStatus != ApproveType.NotModeratedYet)
                        {
                            _logger.LogModeratingByWrongUser(_entityName, id, currentUser.Id,
                                contest.ApprovingModeratorId.GetValueOrDefault(-1), contest.ApprovalStatus);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.ModerationByWrongModeratorErrorName]);
                        }
                        else
                        {
                            ModerationStatus status = await _workspace.ModerateContestAsync(_dbContext, contestRequestForm, contest);
                            _logger.LogModerationStatus(status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForModeration(status, _entityName, id);
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Success(id);
                }
            }
            return Json(response);
        }
    }
}