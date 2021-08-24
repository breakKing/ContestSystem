using ContestSystem.Areas.Contests.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models.Auth;
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
        private readonly ContestsManagerService _contestsManager;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public HomeController(MainDbContext dbContext, ILogger<HomeController> logger, FileStorageService storage, UserManager<User> userManager,
            LocalizerHelperService localizerHelper, ContestsManagerService contestsManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _userManager = userManager;
            _localizerHelper = localizerHelper;
            _contestsManager = contestsManager;

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
            var contestsInfo = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                contestsInfo.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(contestsInfo);
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
            var contestsInfo = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                contestsInfo.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(contestsInfo);
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
            var contestsInfos = new List<ContestBaseInfo>();
            for (int i = 0; i < contests.Count; i++)
            {
                int participantsCount =
                    await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id);
                contestsInfos.Add(ContestBaseInfo.GetFromModel(contests[i], localizers[i], _storage.GetImageInBase64(contests[i].ImagePath)));
            }

            return Json(contestsInfos);
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
                var localizedContest = ContestLocalizedModel.GetFromModel(contest, localizer, _storage.GetImageInBase64(contest.ImagePath),
                                                                            p => _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, culture));

                // Вдруг чаты не созданы (возможно, в будущем надо убрать)
                if (!await _contestsManager.InitialChatsExistAsync(_dbContext, id))
                {
                    await _contestsManager.CreateContestInitialChatsAsync(_dbContext, contest);

                    foreach (var participant in contest.ContestParticipants)
                    {
                        await _contestsManager.AddParticipantToChatsAsync(_dbContext, participant);
                    }
                }

                return Json(localizedContest);
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

            return Json(await _contestsManager.GetContestMonitorAsync(_dbContext, contest));
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

        [HttpGet("{contestId}/problems/{letter}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetProblem(long contestId, char letter)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить задачу {letter} в рамках несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            if (!await _dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == contestId && cp.ParticipantId == currentUser.Id) 
                && !await _dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == contestId && clm.LocalModeratorId == currentUser.Id))
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить задачу {letter} в рамках соревнования с идентификатором {contestId} при отсутствии прав на её получение");
                return BadRequest(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
            }

            var contestProblem = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ContestId == contestId
                                                                                        && cp.Letter == letter);
            if (contestProblem == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить несуществующую задачу {letter} в рамках соревнования с идентификатором {contestId}");
                return NotFound(Constants.ErrorCodes[Constants.ProblemEntityName][Constants.EntityDoesntExistErrorName]);
            }

            return Json(ProblemLocalizedModel.GetFromModel(contestProblem.Problem, 
                _localizerHelper.GetAppropriateLocalizer(contestProblem.Problem.ProblemLocalizers, currentUser.Culture)));
        }

        [HttpGet("{contestId}/rules")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetRulesSet(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            return Json(RulesSetWorkspaceModel.GetFromModel(contest.RulesSet));
        }

        [HttpGet("{contestId}/chats")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetChats(long contestId)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить чаты несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            if (!await _dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == contestId && cp.ParticipantId == currentUser.Id)
                && !await _dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == contestId && clm.LocalModeratorId == currentUser.Id))
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить чаты соревнования с идентификатором {contestId} при отсутствии прав на их получение");
                return BadRequest(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
            }

            var chats = await _contestsManager.GetUserContestChatsAsync(_dbContext, contestId, currentUser.Id);

            return Json(chats);
        }

        [HttpGet("{contestId}/organizers")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetOrganizers(long contestId)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить организаторов несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var organizers = contest.ContestLocalModerators.ConvertAll(ContestOrganizerExternalModel.GetFromModel);

            return Json(organizers);
        }
    }
}
