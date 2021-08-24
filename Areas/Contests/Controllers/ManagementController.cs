using ContestSystem.Areas.Contests.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Contests.Controllers
{
    [Area("Contests")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ManagementController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ContestsManagerService _contestsManager;
        private readonly FileStorageService _storage;
        private readonly LocalizerHelperService _localizerHelper;
        private readonly ILogger<ManagementController> _logger;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public ManagementController(MainDbContext dbContext, UserManager<User> userManager, ContestsManagerService contestsManager,
            FileStorageService storage, LocalizerHelperService localizerHelper, ILogger<ManagementController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _contestsManager = contestsManager;
            _storage = storage;
            _localizerHelper = localizerHelper;
            _logger = logger;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("solutions/{contestId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetAllSolutions(long contestId)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить все решения в рамках несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            if (!await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator) && !await _contestsManager.IsUserContestLocalModeratorAsync(_dbContext, contestId, currentUser.Id))
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить все решения в рамках соревнования с идентификатором {contestId} при отсутствии прав на это");
                return BadRequest(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
            }

            var solutions = await _contestsManager.GetAllContestsSolutionsAsync(_dbContext, contest);
            var imageInBase64 = _storage.GetImageInBase64(contest.ImagePath);
            var contestLocalizer = _localizerHelper.GetAppropriateLocalizer(contest.ContestLocalizers, currentUser.Culture);
            var externalSolutions = solutions.ConvertAll(s => SolutionExternalModel.GetFromModel(s, imageInBase64, contestLocalizer,
                                                                                                _localizerHelper.GetAppropriateLocalizer(s.Problem.ProblemLocalizers, currentUser.Culture), false));

            return Json(externalSolutions);
        }

        [HttpPut("{contestId}/solutions/{solutionId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> ManualSolutionVerdict(long contestId, long solutionId, [FromBody] SolutionManualVerdictForm solutionManualVerdictForm)
        {
            var response = new ResponseObject<SolutionExternalModel>();

            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser(_userManager);
                var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);

                if (!await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator) && !await _contestsManager.IsUserContestLocalModeratorAsync(_dbContext, contestId, currentUser.Id))
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} вручную вынести вердикт для решения с идентификатором {solutionId} " +
                        $"в рамках соревнования с идентификатором {contestId} при отсутствии прав на это");
                    response = ResponseObject<SolutionExternalModel>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    var formStatus = await _contestsManager.CheckSolutionManualVerdictFormAsync(_dbContext, solutionManualVerdictForm);
                    if (formStatus != FormCheckStatus.Correct)
                    {
                        _logger.LogFormCheckStatus(formStatus, Constants.SolutionEntityName, currentUser.Id);
                        response = ResponseObject<SolutionExternalModel>.FormResponseObjectForFormCheck(formStatus, Constants.SolutionEntityName);
                    }
                    else
                    {
                        var editionStatus = await _contestsManager.SetSolutionManualVerdictAsync(_dbContext, solutionManualVerdictForm);
                        _logger.LogEditionStatus(editionStatus, Constants.SolutionEntityName, solutionId, currentUser.Id);
                        if (editionStatus != EditionStatus.Success)
                        {
                            response = ResponseObject<SolutionExternalModel>.FormResponseObjectForEdition(editionStatus, Constants.SolutionEntityName, null);
                        }
                        else
                        {
                            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionManualVerdictForm.SolutionId);
                            var imageInBase64 = _storage.GetImageInBase64(contest.ImagePath);
                            var contestLocalizer = _localizerHelper.GetAppropriateLocalizer(contest.ContestLocalizers, currentUser.Culture);
                            var externalSolution = SolutionExternalModel.GetFromModel(solution, imageInBase64, contestLocalizer,
                                                                                        _localizerHelper.GetAppropriateLocalizer(solution.Problem?.ProblemLocalizers, currentUser.Culture), false);

                            response = ResponseObject<SolutionExternalModel>.FormResponseObjectForEdition(editionStatus, Constants.SolutionEntityName, externalSolution);
                        }
                    }
                }
            }
            else
            {
                response = ResponseObject<SolutionExternalModel>.Fail(ModelState, _entityName);
            }

            return Json(response);
        }
    }
}
