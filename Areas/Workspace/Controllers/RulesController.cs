using ContestSystem.Areas.Workspace.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
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
    public class RulesController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<RulesController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly WorkspaceManagerService _workspace;

        private readonly string _entityName = Constants.RulesSetEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public RulesController(MainDbContext dbContext, ILogger<RulesController> logger, UserManager<User> userManager,
            WorkspaceManagerService workspace)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _workspace = workspace;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("user/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserRules(long userId)
        {
            var rules = await _dbContext.RulesSets.Where(r => r.AuthorId == userId && !r.IsArchieved).ToListAsync();
            var rulesInfo = rules.ConvertAll(RulesSetBaseInfo.GetFromModel);
            return Json(rulesInfo);
        }

        [HttpGet("available/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableRules(long userId)
        {
            var rules = await _dbContext.RulesSets.Where(rs => (rs.AuthorId == userId || rs.IsPublic)
                                                               && !rs.IsArchieved).ToListAsync();
            var rulesInfo = rules.ConvertAll(RulesSetBaseInfo.GetFromModel);
            return Json(rulesInfo);
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedRules(long id)
        {
            var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(r => r.Id == id && !r.IsArchieved);
            if (rules != null)
            {
                var workspaceRules = RulesSetWorkspaceModel.GetFromModel(rules);
                return Json(workspaceRules);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddRules([FromBody] RulesSetForm rulesSetForm)
        {
            var response = new ResponseObject<long>();

            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser(_userManager);
                if (currentUser.Id != rulesSetForm.AuthorId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id,
                        rulesSetForm.AuthorId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    CreationStatusData statusData = await _workspace.CreateRulesSetAsync(_dbContext, rulesSetForm);
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRules([FromBody] RulesSetForm rulesSetForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (id != rulesSetForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId(_entityName, rulesSetForm.Id, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id && !rs.IsArchieved);
                    if (rules == null)
                    {
                        _logger.LogEditingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (currentUser.Id != rules.AuthorId)
                        {
                            _logger.LogEditingByNotAppropriateUser(_entityName, id, currentUser.Id);
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                        }
                        else
                        {
                            EditionStatusData statusData = await _workspace.EditRulesSetAsync(_dbContext, rulesSetForm, rules);
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRules(long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var loadedRules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id && !rs.IsArchieved);
            if (loadedRules == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(_entityName, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                if (currentUser.Id != loadedRules.AuthorId && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
                {
                    _logger.LogDeletingByNotAppropriateUser(_entityName, id, currentUser.Id);
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    DeletionStatus status = await _workspace.DeleteRulesSetAsync(_dbContext, loadedRules);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForDeletion(status, _entityName, id);
                }
            }
            return Json(response);
        }
    }
}
