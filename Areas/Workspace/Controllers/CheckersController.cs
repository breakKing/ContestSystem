using ContestSystem.Areas.Workspace.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
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
    public class CheckersController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly WorkspaceManagerService _workspace;
        private readonly ILogger<CheckersController> _logger;
        private readonly UserManager<User> _userManager;

        private readonly string _entityName = Constants.CheckerEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public CheckersController(MainDbContext dbContext, WorkspaceManagerService workspace,
            ILogger<CheckersController> logger, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _workspace = workspace;
            _logger = logger;
            _userManager = userManager;
            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("user/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCheckers(long userId)
        {
            var checkers = await _dbContext.Checkers.Where(p => p.AuthorId == userId && !p.IsArchieved).ToListAsync();
            var publishedCheckers = checkers.ConvertAll(CheckerBaseInfo.GetFromModel);
            return Json(publishedCheckers);
        }

        [HttpGet("available/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableCheckers(long userId)
        {
            var checkers = await _dbContext.Checkers.Where(c => (c.AuthorId == userId || c.IsPublic)
                                                                && !c.IsArchieved
                                                                && c.ApprovalStatus == ApproveType.Accepted)
                .ToListAsync();
            var publishedCheckers = checkers.ConvertAll(CheckerBaseInfo.GetFromModel);
            return Json(publishedCheckers);
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            if (checker == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var constructedChecker = CheckerWorkspaceModel.GetFromModel(checker);
            return Json(constructedChecker);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddChecker([FromBody] CheckerForm checkerForm)
        {
            var response = new ResponseObject<long>();

            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser(_userManager);
                if (currentUser.Id != checkerForm.AuthorId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id, checkerForm.AuthorId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    CreationStatusData statusData = await _workspace.CreateCheckerAsync(_dbContext, checkerForm);
                    _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForCreation(statusData.Status, _entityName, statusData.Id.GetValueOrDefault(-1));
                }
            }
            else
            {
                response = ResponseObject<long>.Fail(ModelState, _entityName);
            }

            return Json(response);
        }

        [HttpPut("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> EditChecker([FromBody] CheckerForm checkerForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (checkerForm.Id.GetValueOrDefault(-1) != id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId(_entityName, checkerForm.Id, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
                    if (checker == null)
                    {
                        _logger.LogEditingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (checker.AuthorId != currentUser.Id)
                        {
                            _logger.LogEditingByNotAppropriateUser(_entityName, id, currentUser.Id);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                        }
                        else
                        {
                            EditionStatusData statusData = await _workspace.EditCheckerAsync(_dbContext, checkerForm, checker);
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
        public async Task<IActionResult> DeleteChecker(long id)
        {
            var response = new ResponseObject<long>();
            Checker loadedChecker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            if (loadedChecker == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(_entityName, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                if (currentUser.Id != loadedChecker.AuthorId && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
                {
                    _logger.LogDeletingByNotAppropriateUser(_entityName, id, currentUser.Id);
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    DeletionStatus status = await _workspace.DeleteCheckerAsync(_dbContext, loadedChecker);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForDeletion(status, _entityName, id);
                }
            }

            return Json(response);
        }

        [HttpGet("requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetCheckersRequests()
        {
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.NotModeratedYet && !c.IsArchieved).ToListAsync();
            var requests = checkers.ConvertAll(CheckerBaseInfo.GetFromModel);
            return Json(requests);
        }

        [HttpGet("accepted")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetAcceptedCheckers()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Accepted && !c.IsArchieved && c.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = checkers.ConvertAll(CheckerBaseInfo.GetFromModel);
            return Json(requests);
        }

        [HttpGet("rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedCheckers()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Rejected && !c.IsArchieved && c.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = checkers.ConvertAll(CheckerBaseInfo.GetFromModel);
            return Json(requests);
        }

        [HttpPut("{checkerId}/moderate")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ModerateChecker([FromBody] CheckerRequestForm checkerRequestForm, long checkerId)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (checkerRequestForm.CheckerId != checkerId)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId(_entityName, checkerRequestForm.CheckerId, checkerId,
                    currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var checker = await _dbContext.Checkers.FirstOrDefaultAsync(c => c.Id == checkerId && !c.IsArchieved);
                    if (checker == null)
                    {
                        _logger.LogModeratingOfNonExistentEntity(_entityName, checkerId, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (checker.ApprovingModeratorId.GetValueOrDefault(-1) != currentUser.Id && checker.ApprovalStatus != ApproveType.NotModeratedYet)
                        {
                            _logger.LogModeratingByWrongUser(_entityName, checkerId, currentUser.Id, checker.ApprovingModeratorId.GetValueOrDefault(-1), checker.ApprovalStatus);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.ModerationByWrongModeratorErrorName]);
                        }
                        else
                        {
                            ModerationStatus status = await _workspace.ModerateCheckerAsync(_dbContext, checkerRequestForm, checker);
                            _logger.LogModerationStatus(status, _entityName, checkerId, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForModeration(status, _entityName, checkerId);
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
    }
}
