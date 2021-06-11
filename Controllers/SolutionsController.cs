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
                    errors = new List<string> { "Нет решения с таким идентификатором" }
                });
            }
            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId).ToListAsync();
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
                Solution solution = new Solution
                {
                    Code = solutionForm.Code,
                    CompilerGUID = solutionForm.CompilerGUID,
                    ContestId = solutionForm.ContestId,
                    ParticipantId = solutionForm.UserId,
                    ProblemId = solutionForm.ProblemId,
                    SubmitTimeUTC = DateTime.UtcNow,
                    CompilerName = (await _checkerSystemService.GetAvailableCompilersAsync()).FirstOrDefault(c => c.GUID == solutionForm.CompilerGUID).Name,
                    ErrorsMessage = "",
                    Verdict = VerdictType.Undefined,
                    Points = 0
                };
                await _dbContext.Solutions.AddAsync(solution);
                await _dbContext.SaveChangesAsync();
                solution = await _checkerSystemService.CompileSolutionAsync(solution);
                _dbContext.Solutions.Update(solution);
                await _dbContext.SaveChangesAsync();
                return Json(new
                {
                    status = true,
                    message = ""
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

        [HttpPost("{solutionId}/run-test/{testNumber}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> RunTest(long solutionId, short testNumber)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Такого решения не существует" }
                });
            }
            if (!solution.Problem.Tests.Any(t => t.Number == testNumber))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "У данной задачи нет теста с таким номером" }
                });
            }
            if (solution.TestResults.Any(tr => tr.Number == testNumber))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Данное решение уже проверено/проверяется на данном тесте" }
                });
            }
            var testResult = await _checkerSystemService.RunTestForSolutionAsync(solution, testNumber);
            await _dbContext.TestsResults.AddAsync(testResult);
            solution.Verdict = testResult.Verdict;
            solution.Points += testResult.GotPoints;
            _dbContext.Solutions.Update(solution);
            await _dbContext.SaveChangesAsync();
            return Json(testResult);
        }
    }
}
