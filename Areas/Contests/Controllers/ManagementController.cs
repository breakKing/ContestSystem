using ContestSystem.Areas.Contests.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
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
                    $"Попытка от пользователя с идентификатором {currentUser.Id} получить все решения в рамках несуществующего соревнования с идентификатором {contestId}");
                return BadRequest(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var solutions = await _contestsManager.GetAllContestsSolutionsAsync(_dbContext, contest);
            var imageInBase64 = _storage.GetImageInBase64(contest.ImagePath);
            var contestLocalizer = _localizerHelper.GetAppropriateLocalizer(contest.ContestLocalizers, currentUser.Culture);
            var externalSolutions = solutions.ConvertAll(s => SolutionExternalModel.GetFromModel(s, imageInBase64, contestLocalizer,
                                                                                                _localizerHelper.GetAppropriateLocalizer(s.Problem.ProblemLocalizers, currentUser.Culture), false));

            return Json(externalSolutions);
        }
    }
}
