﻿using ContestSystemDbStructure.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.Misc;
using ContestSystem.Models.FormModels;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Logging;
using ContestSystem.Extensions;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly CheckerSystemService _checkerSystemService;
        private readonly VerdicterService _verdicter;
        private readonly FileStorageService _storage;
        private readonly ILogger<SolutionsController> _logger;

        public SolutionsController(MainDbContext dbContext, CheckerSystemService checkerSystemService, VerdicterService verdicter, ILogger<SolutionsController> logger, FileStorageService storage)
        {
            _dbContext = dbContext;
            _checkerSystemService = checkerSystemService;
            _verdicter = verdicter;
            _storage = storage;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedSolution(long id)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == id);
            if (solution == null)
            {
                return NotFound("Такого решения не существует");
            }

            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId)
                .ToListAsync();
            var constructedSolution = ConstructedSolution.GetFromModel(solution, problemsInContest, _storage.GetImageInBase64(solution.Contest.ImagePath));
            return Json(constructedSolution);
        }

        [HttpGet("get-compilers")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetCompilers()
        {
            List<CompilerInfo> compilers = (await _checkerSystemService.GetAvailableCompilersAsync()).ToList();
            return Json(compilers);
        }

        [HttpPost("compile-solution")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> CompileSolution([FromBody] SolutionForm solutionForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (currentUser.Id != solutionForm.UserId)
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} отправить решение в рамках соревнования с идентификатором {solutionForm.ContestId}, когда в форме указан пользователь с идентификатором {solutionForm.UserId}");
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Id текущего пользователя не совпадает с Id пользователя в форме" }
                });
            }
            if (ModelState.IsValid)
            {
                // находим в БД с тем же кодом
                var solution = await _dbContext.Solutions
                    .Where(s => s.ContestId == solutionForm.ContestId)
                    .Where(s => s.ParticipantId == solutionForm.UserId)
                    .Where(s => s.ProblemId == solutionForm.ProblemId)
                    .Where(s => s.CompilerGUID == solutionForm.CompilerGUID)
                    .Where(s => s.Code == solutionForm.Code)
                    .FirstOrDefaultAsync();
                if (solution == null)
                {
                    // добавляем в БД новое
                    solution = new Solution
                    {
                        Code = solutionForm.Code,
                        CompilerGUID = solutionForm.CompilerGUID,
                        ContestId = solutionForm.ContestId,
                        ParticipantId = solutionForm.UserId,
                        ProblemId = solutionForm.ProblemId,
                        SubmitTimeUTC = DateTime.UtcNow,
                        CompilerName = (await _checkerSystemService.GetAvailableCompilersAsync())
                            .FirstOrDefault(c => c.GUID == solutionForm.CompilerGUID)?.Name,
                        ErrorsMessage = "",
                        Verdict = VerdictType.Undefined,
                        Points = 0
                    };
                    await _dbContext.Solutions.AddAsync(solution);
                    await _dbContext.SaveChangesAsync();
                }
                _logger.LogInformation($"На проверку отправлено решение с идентификатором {solution.Id} пользователем {currentUser.Id}");
                // Отправка на компиляцию
                var newSolution = await _checkerSystemService.CompileSolutionAsync(solution);
                solution.ErrorsMessage = newSolution.ErrorsMessage;
                solution.Verdict = newSolution.Verdict;
                _dbContext.Solutions.Update(solution);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogParallelSaveError("Solution", solution.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Ошибка параллельного сохранения" }
                    });
                }
                _logger.LogInformation($"Решение с идентификатором {solution.Id} было успешно скомпилировано");
                return Json(new
                {
                    status = true,
                    data = solution.Id,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [HttpPost("{solutionId}/run-tests")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> RunTests(long solutionId)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId);
            var currentUser = await HttpContext.GetCurrentUser();
            if (solution == null)
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} запустить тесты несуществующего решения с идентификатором {solutionId}");
                return NotFound("Такого решения не существует");
            }

            bool state = true;
            solution.Verdict = VerdictType.TestInProgress;
            _logger.LogInformation($"Пользователем с идентификатором {currentUser.Id} запущено тестирование решения с идентификатором {solutionId}");
            _dbContext.Solutions.Update(solution);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogParallelSaveError("Solution", solutionId);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Ошибка параллельного сохранения" }
                });
            }
            foreach (var test in solution.Problem.Tests.OrderBy(t => t.Number).ToList())
            {
                var result = await RunSingleTest(test, solution);
                if (result.Verdict != VerdictType.Accepted && solution.Contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
                {
                    state = false;
                    break;
                }
            }
            solution.Points = _verdicter.SumPointsForAllTests(solution.TestResults);
            solution.Verdict = _verdicter.GetVerdictForSolution(solution);
            _dbContext.Solutions.Update(solution);
            var contestParticipant = await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ParticipantId == solution.ParticipantId && cp.ContestId == solution.ContestId);
            contestParticipant.Result += _verdicter.GetResultForSolution(solution);
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
                    contestParticipant.Result += _verdicter.GetResultForSolution(solution);
                    _dbContext.ContestsParticipants.Update(contestParticipant);
                }
            }
            _logger.LogInformation($"Решение с идентификатором {solution.Id} успешно протестировано");
            return Json(state);
        }

        protected async Task<TestResult> RunSingleTest(Test test, Solution solution)
        {
            var testResult = solution.TestResults.FirstOrDefault(tr => tr.Number == test.Number);
            if (testResult == null)
            {
                var newTestResult = await _checkerSystemService.RunTestForSolutionAsync(solution, test.Number);
                solution.TestResults.Add(newTestResult);
                await _dbContext.SaveChangesAsync();
                return newTestResult;
            }
            return testResult;
        }
    }
}