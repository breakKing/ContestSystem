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
    public class ProblemsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<ProblemsController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly WorkspaceManagerService _workspace;
        private readonly LocalizerHelperService _localizerHelper;

        private readonly string _entityName = Constants.ProblemEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public ProblemsController(MainDbContext dbContext, ILogger<ProblemsController> logger, UserManager<User> userManager,
            WorkspaceManagerService workspace, LocalizerHelperService localizerHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _workspace = workspace;
            _localizerHelper = localizerHelper;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("user/{userId}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserProblems(long userId, string culture)
        {
            var problems = await _dbContext.Problems.Where(p => p.CreatorId == userId && !p.IsArchieved).ToListAsync();
            var problemsInfo = problems.ConvertAll(p =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, culture);
                var pp = ProblemBaseInfo.GetFromModel(p, localizer);
                return pp;
            });
            return Json(problemsInfo);
        }

        [HttpGet("available/{userId}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableProblems(long userId, string culture)
        {
            var problems = await _dbContext.Problems.Where(p => (p.CreatorId == userId || p.IsPublic)
                                                                && p.ApprovalStatus == ApproveType.Accepted
                                                                && !p.IsArchieved)
                .ToListAsync();
            var problemsInfo = problems.ConvertAll(p =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, culture);
                var pp = ProblemBaseInfo.GetFromModel(p, localizer);
                return pp;
            });
            return Json(problemsInfo);
        }
        
        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedProblem(long id)
        {
            var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id && !p.IsArchieved);
            if (problem != null)
            {
                var workspaceProblem = ProblemWorkspaceModel.GetFromModel(problem);
                return Json(workspaceProblem);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddProblem([FromBody] ProblemForm problemForm)
        {
            if (problemForm.Tests.Sum(t => t.AvailablePoints) != Constants.MaxPointsSumForAllTests)
            {
                ModelState.AddModelError("Tests", $"Sum of available points for all tests is not equal to {Constants.MaxPointsSumForAllTests}");
            }

            var response = new ResponseObject<long>();
            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser(_userManager);
                if (problemForm.CreatorId != currentUser.Id)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id,
                        problemForm.CreatorId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    if (!await _dbContext.Checkers.AnyAsync(ch => ch.Id == problemForm.CheckerId && !ch.IsArchieved))
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} создать сущность \"{_entityName}\" " +
                            $"с использованием несуществующей сущности \"{Constants.CheckerEntityName}\" с идентификатором {problemForm.CheckerId}");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.CheckerEntityName][Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        var statusData = await _workspace.CreateProblemAsync(_dbContext, problemForm, currentUser.IsLimitedInProblems);
                        _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id.GetValueOrDefault(-1).ToString(), currentUser.Id);
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
        public async Task<IActionResult> EditProblem([FromBody] ProblemForm problemForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (id != problemForm.Id.GetValueOrDefault(-1))
            {
                _logger.LogEditingWithNonEqualFormAndRequestId(_entityName, problemForm.Id, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (problemForm.Tests.Sum(t => t.AvailablePoints) != Constants.MaxPointsSumForAllTests)
                {
                    ModelState.AddModelError("Tests", $"Sum of available points for all tests is not equal to {Constants.MaxPointsSumForAllTests}");
                }

                if (ModelState.IsValid)
                {
                    var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id && !p.IsArchieved);
                    if (problem == null)
                    {
                        _logger.LogEditingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (currentUser.Id != problem.CreatorId)
                        {
                            _logger.LogEditingByNotAppropriateUser(_entityName, id, currentUser.Id);
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                        }
                        else
                        {
                            EditionStatusData statusData = await _workspace.EditProblemAsync(_dbContext, problemForm, problem);
                            _logger.LogEditionStatus(statusData.Status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForEdition(statusData.Status, _entityName, statusData.Id.GetValueOrDefault(-1));
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
        public async Task<IActionResult> DeletePost(long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var loadedProblem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id && !p.IsArchieved);
            if (loadedProblem == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(_entityName, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                if (currentUser.Id != loadedProblem.CreatorId && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
                {
                    _logger.LogDeletingByNotAppropriateUser(_entityName, id, currentUser.Id);
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    DeletionStatus status = await _workspace.DeleteProblemAsync(_dbContext, loadedProblem);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForDeletion(status, _entityName, id);
                }
            }
            return Json(response);
        }

        [HttpGet("requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetProblemsRequests()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var problems = await _dbContext.Problems
                .Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet && !p.IsArchieved).ToListAsync();
            var requests = problems.ConvertAll(p => ProblemBaseInfo.GetFromModel(p, _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, currentUser.Culture)));
            return Json(requests);
        }

        [HttpGet("accepted")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetAcceptedProblems()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var problems = await _dbContext.Problems
                .Where(p => p.ApprovalStatus == ApproveType.Accepted && !p.IsArchieved
                            && p.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = problems.ConvertAll(p => ProblemBaseInfo.GetFromModel(p, _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, currentUser.Culture)));
            return Json(requests);
        }

        [HttpGet("rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedProblems()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var problems = await _dbContext.Problems
                .Where(p => p.ApprovalStatus == ApproveType.Rejected && !p.IsArchieved
                            && p.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = problems.ConvertAll(p => ProblemBaseInfo.GetFromModel(p, _localizerHelper.GetAppropriateLocalizer(p.ProblemLocalizers, currentUser.Culture)));
            return Json(requests);
        }

        [HttpPut("{id}/moderate")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ModerateProblem([FromBody] ProblemRequestForm problemRequestForm,
            long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (problemRequestForm.ProblemId != id)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId(_entityName, problemRequestForm.ProblemId, id,
                    currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id && !p.IsArchieved);
                    if (problem == null)
                    {
                        _logger.LogModeratingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (problem.ApprovingModeratorId.GetValueOrDefault(-1) != currentUser.Id && problem.ApprovalStatus != ApproveType.NotModeratedYet)
                        {
                            _logger.LogModeratingByWrongUser(_entityName, id, currentUser.Id, problem.ApprovingModeratorId.GetValueOrDefault(-1), problem.ApprovalStatus);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.ModerationByWrongModeratorErrorName]);
                        }
                        else
                        {
                            ModerationStatus status = await _workspace.ModerateProblemAsync(_dbContext, problemRequestForm, problem);
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
