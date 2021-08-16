using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Contests.Controllers
{
    [Area("Contests")]
    [Route("api/[area]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly FileStorageService _storage;
        private readonly UserManager<User> _userManager;
        private readonly LocalizerHelperService _localizerHelper;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public HomeController(MainDbContext dbContext, ILogger<HomeController> logger, FileStorageService storage, UserManager<User> userManager,
            LocalizerHelperService localizerHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _userManager = userManager;
            _localizerHelper = localizerHelper;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("upcoming/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUpcomingContests(string culture) //TODO: ulong? offset = null, ulong? count = null
        {
            DateTime now = DateTime.UtcNow;
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC > now
                                                                && c.IsPublic
                                                                && c.ContestParticipants.All(cp =>
                                                                    cp.ParticipantId != currentUser.Id)
                                                                && c.ContestLocalModerators.All(cp =>
                                                                    cp.LocalModeratorId != currentUser.Id))
                .ToListAsync();
            var localizers = contests.ConvertAll(c => _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, culture));
            var publishedContests = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("running/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetRunningContests(string culture) //TODO: ulong? offset = null, ulong? count = null
        {
            DateTime now = DateTime.UtcNow;
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.StartDateTimeUTC <= now
                                                                && c.StartDateTimeUTC.AddMinutes(c.DurationInMinutes) >
                                                                now
                                                                && c.RulesSet.PublicMonitor)
                .ToListAsync();
            var localizers = contests.ConvertAll(c => _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, culture));
            var publishedContests = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(publishedContests);
        }

        [HttpGet("participating/{culture}")]
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
            var localizers = contests.ConvertAll(c => _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, culture));
            var publishedContests = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                publishedContests.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
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
                var localizer = _localizerHelper.GetAppropriateLocalizer(contest.ContestLocalizers, culture);
                if (localizer == null)
                {
                    return NotFound(_errorCodes[Constants.EntityLocalizerDoesntExistErrorName]);
                }

                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contest.Id);
                var publishedContest = ContestLocalizedModel.GetFromModel(contest, localizer, _storage.GetImageInBase64(contest.ImagePath),
                                                                            p => _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, culture));
                return Json(publishedContest);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpGet("{contestId}/monitor")]
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

        [HttpGet("{contestId}/solutions/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetUserSolutions(long contestId, long userId)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
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
                SolutionBaseInfo.GetFromModel(s, _localizerHelper.GetAppropriateLocalizer(s.Problem.ProblemLocalizers, currentUser.Culture))));
        }

        // TODO: перепелить метод
        /*[HttpGet("published/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedProblem(long id, string culture) 
        {
            var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id && !p.IsArchieved);
            if (problem != null)
            {
                var localizer = problem.ProblemLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return NotFound(Constants.ErrorCodes[Constants.ProblemEntityName][Constants.EntityLocalizerDoesntExistErrorName]);
                }

                var publishedProblem = PublishedProblem.GetFromModel(problem, localizer);
                return Json(publishedProblem);
            }

            return NotFound(Constants.ErrorCodes[Constants.ProblemEntityName][Constants.EntityDoesntExistErrorName]);
        }*/
    }
}
