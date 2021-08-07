using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.Misc;
using Microsoft.AspNetCore.Identity;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("get-user-checkers/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCheckers(long id)
        {
            var checkers = await _dbContext.Checkers.Where(p => p.AuthorId == id && !p.IsArchieved).ToListAsync();
            var publishedCheckers = checkers.ConvertAll(PublishedChecker.GetFromModel);
            return Json(publishedCheckers);
        }

        [HttpGet("get-available-checkers/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableCheckers(long id)
        {
            var checkers = await _dbContext.Checkers.Where(c => (c.AuthorId == id || c.IsPublic)
                                                                && !c.IsArchieved
                                                                && c.ApprovalStatus == ApproveType.Accepted)
                .ToListAsync();
            var publishedCheckers = checkers.ConvertAll(PublishedChecker.GetFromModel);
            return Json(publishedCheckers);
        }

        [HttpGet("published/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            if (checker == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var publishedChecker = PublishedChecker.GetFromModel(checker);
            return Json(publishedChecker);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            if (checker == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var constructedChecker = ConstructedChecker.GetFromModel(checker);
            return Json(constructedChecker);
        }

        [HttpPost("add-checker")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddChecker([FromBody] CheckerForm checkerForm)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (ModelState.IsValid)
            {
                if (currentUser.Id != checkerForm.AuthorId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id, checkerForm.AuthorId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    CreationStatusData statusData = await _workspace.CreateCheckerAsync(_dbContext, checkerForm);
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

        [HttpPut("edit-checker/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> EditChecker([FromBody] CheckerForm checkerForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (checkerForm.Id == null || checkerForm.Id.Value != id)
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
                            EditionStatus status = await _workspace.EditCheckerAsync(_dbContext, checkerForm, checker);
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
        [HttpDelete("delete-checker/{id}")]
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


        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetCheckersRequests()
        {
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.NotModeratedYet && !c.IsArchieved).ToListAsync();
            var requests = checkers.ConvertAll(ConstructedChecker.GetFromModel);
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedCheckers()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Accepted && !c.IsArchieved && c.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = checkers.ConvertAll(ConstructedChecker.GetFromModel);
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedCheckers()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Rejected && !c.IsArchieved && c.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                .ToListAsync();
            var requests = checkers.ConvertAll(ConstructedChecker.GetFromModel);
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectChecker([FromBody] CheckerRequestForm checkerRequestForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (checkerRequestForm.CheckerId != id)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId(_entityName, checkerRequestForm.CheckerId, id,
                    currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var checker = await _dbContext.Checkers.FirstOrDefaultAsync(c => c.Id == id && !c.IsArchieved);
                    if (checker == null)
                    {
                        _logger.LogModeratingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (checker.ApprovingModeratorId.GetValueOrDefault(-1) != currentUser.Id && checker.ApprovalStatus != ApproveType.NotModeratedYet)
                        {
                            _logger.LogModeratingByWrongUser(_entityName, id, currentUser.Id, checker.ApprovingModeratorId.GetValueOrDefault(-1), checker.ApprovalStatus);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.ModerationByWrongModeratorErrorName]);
                        }
                        else
                        {
                            ModerationStatus status = await _workspace.ModerateCheckerAsync(_dbContext, checkerRequestForm, checker);
                            _logger.LogModerationStatus(status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForModeration(status, _entityName, id);
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