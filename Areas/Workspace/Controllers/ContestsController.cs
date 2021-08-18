using ContestSystem.Areas.Workspace.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Workspace.Controllers
{
    [Area("Workspace")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ContestsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<ContestsController> _logger;
        private readonly FileStorageService _storage;
        private readonly WorkspaceManagerService _workspace;
        private readonly UserManager<User> _userManager;
        private readonly LocalizerHelperService _localizerHelper;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public ContestsController(MainDbContext dbContext, ILogger<ContestsController> logger, FileStorageService storage, WorkspaceManagerService workspace, 
            UserManager<User> userManager, LocalizerHelperService localizerHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _workspace = workspace;
            _userManager = userManager;
            _localizerHelper = localizerHelper;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetContest(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
            if (contest != null)
            {
                var problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contest.Id).ToListAsync();
                var workspaceContest =
                    ContestWorkspaceModel.GetFromModel(contest, _storage.GetImageInBase64(contest.ImagePath), 
                    p => _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, currentUser.Culture));
                return Json(workspaceContest);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpGet("user/{userId}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserContests(long userId, string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.CreatorId == userId).ToListAsync();
            var contestsInfo = contests.ConvertAll(c =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, culture);
                var ci = ContestBaseInfo.GetFromModel(c, localizer, _storage.GetImageInBase64(c.ImagePath));
                return ci;
            });
            return Json(contestsInfo);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddContest([FromForm] ContestForm contestForm)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var response = new ResponseObject<long>();
            if (ModelState.IsValid)
            {
                if (currentUser.Id != contestForm.CreatorUserId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id, contestForm.CreatorUserId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    bool allTasksExist = true;
                    if (contestForm.Problems.Count > 0)
                    {
                        allTasksExist = !contestForm.Problems.Any(pf => !_dbContext.Problems.AnyAsync(p => p.Id == pf.ProblemId && !p.IsArchieved)
                                                                                            .GetAwaiter().GetResult());
                    }
                    if (!await _dbContext.RulesSets.AnyAsync(rs => rs.Id == contestForm.RulesSetId && !rs.IsArchieved))
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} создать сущность \"{_entityName}\" " +
                            $"с использованием несуществующей сущности \"{Constants.RulesSetEntityName}\" с идентификатором {contestForm.RulesSetId}");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.RulesSetEntityName][Constants.EntityDoesntExistErrorName]);
                    }
                    else if (!allTasksExist)
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} создать сущность \"{_entityName}\" " +
                            $"с использованием одной или нескольких несуществующих сущностей \"{Constants.ProblemEntityName}\"");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.ProblemEntityName][Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        CreationStatusData statusData = await _workspace.CreateContestAsync(_dbContext, contestForm, currentUser.IsLimitedInContests);
                        _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id, currentUser.Id);
                        response = ResponseObject<long>.FormResponseObjectForCreation(statusData.Status, _entityName, statusData.Id.GetValueOrDefault(-1));
                    }
                }
            }
            else
            {
                response = ResponseObject<long>.Fail(ModelState, _entityName);
            }
            return Json(response);
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditContest([FromForm] ContestForm contestForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var response = new ResponseObject<long>();
            if (id != contestForm.Id.GetValueOrDefault(-1))
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
                            EditionStatus status = await _workspace.EditContestAsync(_dbContext, contestForm, contest);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContest(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var response = new ResponseObject<long>();
            Contest loadedContest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
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
                    DeletionStatus status = await _workspace.DeleteContestAsync(_dbContext, loadedContest);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForDeletion(status, _entityName, id);
                }
            }
            return Json(response);
        }

        [HttpGet("requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetContestsRequests()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.NotModeratedYet)
                .ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, currentUser.Culture);
                var cr = ContestBaseInfo.GetFromModel(c, localizer, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("accepted")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetAcceptedContests()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                                && c.ApprovingModeratorId.GetValueOrDefault(-1) ==
                                                                currentUser.Id)
                .ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, currentUser.Culture);
                var cr = ContestBaseInfo.GetFromModel(c, localizer, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedContests()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Rejected
                                                                && c.ApprovingModeratorId.GetValueOrDefault(-1) ==
                                                                currentUser.Id)
                .ToListAsync();
            var requests = contests.ConvertAll(c =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(c.ContestLocalizers, currentUser.Culture);
                var cr = ContestBaseInfo.GetFromModel(c, localizer, _storage.GetImageInBase64(c.ImagePath));
                return cr;
            });
            return Json(requests);
        }

        [HttpPut("{id}/moderate")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ModerateContest([FromBody] ContestRequestForm contestRequestForm,
            long id)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var response = new ResponseObject<long>();
            if (contestRequestForm.ContestId != id)
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
