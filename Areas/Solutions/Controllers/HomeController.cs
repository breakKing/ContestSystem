using ContestSystem.Areas.Solutions.Services;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("api/[area]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SolutionsManagerService _solutionsManager;
        private readonly FileStorageService _storage;
        private readonly ILogger<HomeController> _logger;
        private readonly NotifierService _notifier;
        private readonly LocalizerHelperService _localizerHelper;

        private readonly string _entityName = Constants.SolutionEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public HomeController(MainDbContext dbContext, UserManager<User> userManager,
            SolutionsManagerService solutionsManager, ILogger<HomeController> logger, FileStorageService storage,
            NotifierService notifier, LocalizerHelperService localizerHelper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _solutionsManager = solutionsManager;
            _storage = storage;
            _logger = logger;
            _notifier = notifier;
            _localizerHelper = localizerHelper;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedSolution(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == id);
            if (solution == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId)
                .ToListAsync();
            var constructedSolution = SolutionExternalModel.GetFromModel(solution,
                _storage.GetImageInBase64(solution.Contest.ImagePath),
                _localizerHelper.GetAppropriateLocalizer(solution.Contest.ContestLocalizers, currentUser.Culture),
                _localizerHelper.GetAppropriateLocalizer(solution.Problem.ProblemLocalizers, currentUser.Culture));
            return Json(constructedSolution);
        }

        [HttpGet("compilers")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetCompilers()
        {
            List<CompilerInfo> compilers = await _solutionsManager.GetCompilersAsync(_dbContext);
            if (compilers == null)
            {
                return NotFound(Constants.ErrorCodes[Constants.CommonSectionName][Constants.CheckerServersUnavailableErrorName]);
            }
            return Json(compilers);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> SendSolution([FromBody] SolutionForm solutionForm)
        {
            var response = new ResponseObject<long>();

            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (currentUser.Id != solutionForm.UserId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} отправить решение в рамках соревнования с идентификатором {solutionForm.ContestId}, когда в форме указан пользователь с идентификатором {solutionForm.UserId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var formStatus = await _solutionsManager.CheckSolutionFormAsync(_dbContext, solutionForm);
                    if (formStatus != FormCheckStatus.Correct)
                    {
                        _logger.LogFormCheckStatus(formStatus, _entityName, currentUser.Id);
                        response = ResponseObject<long>.FormResponseObjectForFormCheck(formStatus, _entityName);
                    }
                    else
                    {
                        var statusData = await _solutionsManager.CreateSolutionAsync(_dbContext, solutionForm);
                        _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id, currentUser.Id);
                        response = ResponseObject<long>.FormResponseObjectForCreation(statusData.Status, _entityName, statusData.Id.GetValueOrDefault(-1));
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, _entityName);
                }
            }

            return Json(response);
        }

        [HttpPost("{solutionId}/compile")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> CompileSolution(long solutionId)
        {
            var response = new ResponseObject<long>();

            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} скомпилировать несуществующее решение с идентификатором {solutionId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                bool isUserLocalModerator = await _dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == solution.ContestId
                                                                                                        && clm.LocalModeratorId == currentUser.Id);

                if (currentUser.Id != solution.ParticipantId && !isUserLocalModerator)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} скомпилировать решение с идентификатором {solutionId}, не имея на это прав");
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    _logger.LogInformation(
                    $"На компиляцию отправлено решение с идентификатором {solution.Id} пользователем {currentUser.Id}");
                    solution = await _solutionsManager.CompileSolutionAsync(_dbContext, solution);
                    if (solution == null)
                    {
                        _logger.LogError($"При компиляции решения с идентификатором {solution.Id} пользователем {currentUser.Id} произошла серьёзная ошибка");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        if (solution.Verdict != VerdictType.CheckerServersUnavailable)
                        {
                            _logger.LogInformation($"Решение с идентификатором {solution.Id} прошло процедуру компиляции");
                        }
                        var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == solution.ContestId);
                        await _notifier.UpdateOnSolutionActualResultAsync(contest, solution);
                        response = ResponseObject<long>.Success(solutionId);
                    }
                }
            }

            return Json(response);
        }

        [HttpPost("{solutionId}/run")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> RunTests(long solutionId)
        {
            var response = new ResponseObject<long>();

            var solution = await _dbContext.Solutions.Include(s => s.Problem)
                                                        .ThenInclude(p => p.Checker)
                                                        .FirstOrDefaultAsync(s => s.Id == solutionId);
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            if (solution == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} запустить тесты несуществующего решения с идентификатором {solutionId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                bool isUserLocalModerator = await _dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == solution.ContestId
                                                                                                        && clm.LocalModeratorId == currentUser.Id);

                if (currentUser.Id != solution.ParticipantId && !isUserLocalModerator)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} запустить тесты решения с идентификатором {solutionId}, не имея на это прав");
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    solution.Verdict = VerdictType.TestInProgress;
                    
                    _dbContext.Solutions.Update(solution);

                    bool saveSuccess = await _dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        _logger.LogWarning($"Пользователем с идентификатором {currentUser.Id} была осуществлена попытка запуска тестирования решения" +
                            $" с идентификатором {solutionId}, но при сохранении информации в базу данных произошла ошибка");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName]);
                    }
                    else
                    {
                        _logger.LogInformation(
                        $"Пользователем с идентификатором {currentUser.Id} запущено тестирование решения с идентификатором {solutionId}");
                        await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);

                        solution = await _solutionsManager.TestSolutionAsync(_dbContext, solution);

                        if (solution != null)
                        {
                            await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
                            _logger.LogInformation($"Решение с идентификатором {solution.Id} успешно протестировано");
                            response = ResponseObject<long>.Success(solutionId);
                        }
                        else
                        {
                            _logger.LogError($"При тестировании решения с идентификатором {solution.Id} возникла серьёзная ошибка");
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                        }
                    }
                }
            }

            return Json(response);
        }
    }
}
