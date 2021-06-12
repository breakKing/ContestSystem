using ContestSystemDbStructure.Models;
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

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly CheckerSystemService _checkerSystemService;

        public SolutionsController(MainDbContext dbContext, CheckerSystemService checkerSystemService)
        {
            _dbContext = dbContext;
            _checkerSystemService = checkerSystemService;
        }

        [HttpGet("{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedSolution(long id)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == id);
            if (solution == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Нет решения с таким идентификатором"}
                });
            }

            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId)
                .ToListAsync();
            var constructedSolution = SolutionExternalModel.GetFromModel(solution, problemsInContest);
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
            if (ModelState.IsValid)
            {
                // находим собранное с тем же кодом
                var solution = await _dbContext.Solutions
                    .Where(s => s.ContestId == solutionForm.ContestId)
                    .Where(s => s.ParticipantId == solutionForm.UserId)
                    .Where(s => s.ProblemId == solutionForm.ProblemId)
                    .Where(s => s.CompilerGUID == solutionForm.CompilerGUID)
                    .Where(s => s.Code == solutionForm.Code)
                    .FirstOrDefaultAsync();
                if (solution == null)
                {
                    // компиляция нового
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

                // проверка что файл реально собран
                solution = await _checkerSystemService.CompileSolutionAsync(solution);
                _dbContext.Solutions.Update(solution);
                await _dbContext.SaveChangesAsync();
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
        public async Task<IActionResult> RunTest(long solutionId)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
            {
                return NotFound("Такого решения не существует");
            }

            bool state = true;
            foreach (var test in solution.Problem.Tests)
            {
                var result = await this.RunSingleTest(test, solution);
                if (result.Verdict != VerdictType.Accepted)
                {
                    state = false;
                    break;
                }
            }


            return Json(state);
        }

        protected async Task<TestResult> RunSingleTest(Test test, Solution solution)
        {
            var testResult = solution.TestResults.FirstOrDefault(tr => tr.Number == test.Number);
            if (testResult == null)
            {
                testResult = await _checkerSystemService.RunTestForSolutionAsync(solution, test.Number);
                await _dbContext.TestsResults.AddAsync(testResult);
                solution.Verdict = testResult.Verdict;
                solution.Points += testResult.GotPoints;
                _dbContext.Solutions.Update(solution);
                await _dbContext.SaveChangesAsync();
            }

            return testResult;
        }
    }
}