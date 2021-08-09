using ContestSystem.Areas.Solutions.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly CheckerSystemService _checkerSystemService;
        private readonly SolutionsManagerService _solutionsManager;
        private readonly FileStorageService _storage;
        private readonly ILogger<HomeController> _logger;
        private readonly NotifierService _notifier;

        public HomeController(MainDbContext dbContext, CheckerSystemService checkerSystemService,
            SolutionsManagerService solutionsManager, ILogger<HomeController> logger, FileStorageService storage,
            NotifierService notifier)
        {
            _dbContext = dbContext;
            _checkerSystemService = checkerSystemService;
            _solutionsManager = solutionsManager;
            _storage = storage;
            _logger = logger;
            _notifier = notifier;
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedSolution(long id)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == id);
            if (solution == null)
            {
                return NotFound("Такого решения не существует");
            }

            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId)
                .ToListAsync();
            var constructedSolution = ConstructedSolution.GetFromModel(solution, problemsInContest,
                _storage.GetImageInBase64(solution.Contest.ImagePath));
            return Json(constructedSolution);
        }

        [HttpGet("compilers")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetCompilers()
        {
            List<CompilerInfo> compilers = await _checkerSystemService.GetAvailableCompilersAsync(_dbContext);
            if (compilers == null)
            {
                return NotFound("Сервера проверки временно недоступны");
            }
            return Json(compilers);
        }

        [HttpPost("")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> SendSolution([FromBody] SolutionForm solutionForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (currentUser.Id != solutionForm.UserId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} отправить решение в рамках соревнования с идентификатором {solutionForm.ContestId}, когда в форме указан пользователь с идентификатором {solutionForm.UserId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Id текущего пользователя не совпадает с Id пользователя в форме" }
                });
            }

            if (ModelState.IsValid)
            {
                // находим в БД с тем же кодом
                var solution = await _dbContext.Solutions.Where(s => s.ContestId == solutionForm.ContestId
                                                                     && s.ParticipantId == solutionForm.UserId
                                                                     && s.ProblemId == solutionForm.ProblemId
                                                                     && s.CompilerGUID == solutionForm.CompilerGUID
                                                                     && s.Code == solutionForm.Code)
                    .FirstOrDefaultAsync();
                if (solution == null)
                {
                    var compilers = await _checkerSystemService.GetAvailableCompilersAsync(_dbContext);
                    if (compilers == null)
                    {
                        _logger.LogWarning($"На проверку было отправлено решение с компилятором с идентификатором \"{solutionForm.CompilerGUID}\" в рамках соревнования с идентификатором {solutionForm.ContestId} для задачи {solutionForm.ProblemId}, но сервера проверки недоступны");
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Сервера проверки временно недоступны" }
                        });
                    }
                    string compilerName = compilers.FirstOrDefault(c => c.GUID == solutionForm.CompilerGUID)?.Name ?? null;
                    if (compilerName == null)
                    {
                        _logger.LogWarning($"На проверку было отправлено решение с компилятором с идентификатором \"{solutionForm.CompilerGUID}\" в рамках соревнования с идентификатором {solutionForm.ContestId} для задачи {solutionForm.ProblemId}, но такого компилятора нет в списке доступных");
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Такого компилятора не существует" }
                        });
                    }
                    // добавляем в БД новое
                    solution = new Solution
                    {
                        Code = solutionForm.Code,
                        CompilerGUID = solutionForm.CompilerGUID,
                        ContestId = solutionForm.ContestId,
                        ParticipantId = solutionForm.UserId,
                        ProblemId = solutionForm.ProblemId,
                        SubmitTimeUTC = DateTime.UtcNow,
                        CompilerName = compilerName,
                        ErrorsMessage = "",
                        Verdict = VerdictType.Undefined,
                        Points = 0
                    };
                    await _dbContext.Solutions.AddAsync(solution);
                    await _dbContext.SaveChangesAsync();
                    return Json(new
                    {
                        status = true,
                        data = solution.Id
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Такое решение уже имеется" }
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [HttpPost("{solutionId}/compile")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> CompileSolution(long solutionId)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} скомпилировать несуществующее решение с идентификатором {solutionId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Такого решения не существует" }
                });
            }

            if (currentUser.Id != solution.ParticipantId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} скомпилировать не своё решение с идентификатором {solutionId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка скомпилировать не своё решение" }
                });
            }

            _logger.LogInformation(
                $"На компиляцию отправлено решение с идентификатором {solution.Id} пользователем {currentUser.Id}");
            // Отправка на компиляцию
            var newSolution = await _checkerSystemService.CompileSolutionAsync(_dbContext, solution);
            if (newSolution == null)
            {
                solution.Verdict = VerdictType.CheckerServersUnavailable;
            }
            else
            {
                solution.ErrorsMessage = newSolution.ErrorsMessage;
                solution.Verdict = newSolution.Verdict;
            }
            _dbContext.Solutions.Update(solution);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogDbSaveError("Solution", solution.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Ошибка параллельного сохранения" }
                });
            }
            if (solution.Verdict != VerdictType.CheckerServersUnavailable)
            {
                _logger.LogInformation($"Решение с идентификатором {solution.Id} прошло процедуру компиляции");
            }
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == solution.ContestId);
            await _notifier.UpdateOnSolutionActualResultAsync(contest, solution);
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }

        [HttpPost("{solutionId}/run")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> RunTests(long solutionId)
        {
            var solution = await _dbContext.Solutions.Include(s => s.Problem)
                .ThenInclude(p => p.Checker)
                .FirstOrDefaultAsync(s => s.Id == solutionId);
            var currentUser = await HttpContext.GetCurrentUser();
            if (solution == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} запустить тесты несуществующего решения с идентификатором {solutionId}");
                return NotFound("Такого решения не существует");
            }

            bool state = true;
            solution.Verdict = VerdictType.TestInProgress;
            _logger.LogInformation(
                $"Пользователем с идентификатором {currentUser.Id} запущено тестирование решения с идентификатором {solutionId}");
            _dbContext.Solutions.Update(solution);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogDbSaveError("Solution", solutionId);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Ошибка параллельного сохранения" }
                });
            }

            await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
            foreach (var test in solution.Problem.Tests.OrderBy(t => t.Number).ToList())
            {
                var result = await RunSingleTest(test, solution);
                if (result == null)
                {
                    state = false;
                    break;
                }
                if (result.Verdict != VerdictType.Accepted && solution.Contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
                {
                    state = false;
                    break;
                }
            }
            if (state)
            {
                solution.Verdict = _solutionsManager.GetVerdictForSolution(solution, solution.Contest.RulesSet);
                _dbContext.Solutions.Update(solution);
                var contestParticipant = await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ParticipantId == solution.ParticipantId && cp.ContestId == solution.ContestId);
                var otherSolutions = await _dbContext.Solutions.Where(s => s.ParticipantId == solution.ParticipantId
                                                                            && s.ContestId == solution.ContestId
                                                                            && s.ProblemId == solution.ProblemId
                                                                            && s.Id != solution.Id)
                                                                .Include(s => s.Contest)
                                                                .ThenInclude(c => c.RulesSet)
                                                                .ToListAsync();
                contestParticipant.Result += _solutionsManager.GetAdditionalResultForSolutionSubmit(otherSolutions, solution, solution.Contest.RulesSet);
                _dbContext.ContestsParticipants.Update(contestParticipant);
                bool saved = false;
                while (!saved)
                {
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        contestParticipant = await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ParticipantId == solution.ParticipantId && cp.ContestId == solution.ContestId);
                        otherSolutions = await _dbContext.Solutions.Where(s => s.ParticipantId == solution.ParticipantId
                                                                            && s.ContestId == solution.ContestId
                                                                            && s.ProblemId == solution.ProblemId
                                                                            && s.Id != solution.Id)
                                                                .Include(s => s.Contest)
                                                                .ThenInclude(c => c.RulesSet)
                                                                .ToListAsync();
                        contestParticipant.Result += _solutionsManager.GetAdditionalResultForSolutionSubmit(otherSolutions, solution, solution.Contest.RulesSet);
                        _dbContext.ContestsParticipants.Update(contestParticipant);
                    }
                }
                await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
                _logger.LogInformation($"Решение с идентификатором {solution.Id} успешно протестировано");
            }
            return Json(state);
        }

        protected async Task<TestResult> RunSingleTest(Test test, Solution solution)
        {
            var testResult = solution.TestResults.FirstOrDefault(tr => tr.Number == test.Number);
            if (testResult == null)
            {
                var newTestResult = await _checkerSystemService.RunTestForSolutionAsync(_dbContext, solution, test.Number);
                if (newTestResult == null)
                {
                    solution.Verdict = VerdictType.CheckerServersUnavailable;
                }
                else
                {
                    solution.Points += newTestResult.GotPoints;
                    solution.TestResults.Add(newTestResult);
                }
                _dbContext.Solutions.Update(solution);
                await _dbContext.SaveChangesAsync();
                await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
                return newTestResult;
            }

            return testResult;
        }
    }
}
