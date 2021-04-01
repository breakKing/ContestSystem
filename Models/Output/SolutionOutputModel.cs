using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class SolutionOutputModel : IOutputModel<SolutionBaseModel>
    {
        private readonly IStringLocalizer<SolutionOutputModel> _localizer;
        private readonly MainDbContext _dbContext;

        public string Alias { get; set; }
        public string ProblemName { get; set; }
        public string Compiler { get; set; }
        public string Code { get; set; }
        public string ErrorsMessage { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string Verdict { get; set; }
        public short Points { get; set; }
        public List<TestResultEntryOutputModel> TestsResults { get; set; } = new List<TestResultEntryOutputModel>();

        public SolutionOutputModel(IStringLocalizer<SolutionOutputModel> localizer, MainDbContext dbContext)
        {
            _localizer = localizer;
            _dbContext = dbContext;
        }

        public void TransformForOutput(SolutionBaseModel baseModel)
        {
            ContestProblemBaseModel contestProblem = _dbContext.ContestsProblems.FirstOrDefault(cp => cp.ContestId == baseModel.ContestId
                                                                                                           && cp.ProblemId == baseModel.ProblemId);

            Alias = contestProblem.Alias;
            ProblemName = baseModel.Problem.Name;
            Compiler = baseModel.Compiler;
            SubmitTimeUTC = baseModel.SubmitTimeUTC;
            Code = baseModel.Code;
            ErrorsMessage = baseModel.ErrorsMessage;
            Points = baseModel.Points;
            Verdict = baseModel.Verdict switch
            {
                VerdictType.CompilationError => _localizer["CompilationError"],
                VerdictType.CompilationSucceed => _localizer["CompilationSucceed"],
                VerdictType.PresentationError => _localizer["PresentationError"],
                VerdictType.RuntimeError => _localizer["RuntimeError"],
                VerdictType.WrongAnswer => _localizer["WrongAnswer"],
                VerdictType.TimeLimitExceeded => _localizer["TimeLimitExceeded"],
                VerdictType.MemoryLimitExceeded => _localizer["MemoryLimitExceeded"],
                VerdictType.UnexpectedError => _localizer["UnexpectedError"],
                VerdictType.PartialSolution => _localizer["PartialSolution"],
                VerdictType.Accepted => _localizer["Accepted"],
                VerdictType.TestInProgress => _localizer["TestInProgress"],
                VerdictType.Undefined => "Undefined",
                _ => "",
            };

            TestsResults = _dbContext.TestsResults.Where(tr => tr.SolutionId == baseModel.Id)
                                                    .ToList()
                                                    .ConvertAll(tr =>
                                                    {
                                                        TestResultEntryOutputModel trOut = new TestResultEntryOutputModel(_localizer);
                                                        trOut.TransformForOutput(tr);
                                                        return trOut;
                                                    });
        }

        public async Task TransformForOutputAsync(SolutionBaseModel baseModel)
        {
            ContestProblemBaseModel contestProblem = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ContestId == baseModel.ContestId
                                                                                                                    && cp.ProblemId == baseModel.ProblemId);

            Alias = contestProblem.Alias;
            ProblemName = baseModel.Problem.Name;
            Compiler = baseModel.Compiler;
            SubmitTimeUTC = baseModel.SubmitTimeUTC;
            Code = baseModel.Code;
            ErrorsMessage = baseModel.ErrorsMessage;
            Points = baseModel.Points;
            Verdict = baseModel.Verdict switch
            {
                VerdictType.CompilationError => _localizer["CompilationError"],
                VerdictType.CompilationSucceed => _localizer["CompilationSucceed"],
                VerdictType.PresentationError => _localizer["PresentationError"],
                VerdictType.RuntimeError => _localizer["RuntimeError"],
                VerdictType.WrongAnswer => _localizer["WrongAnswer"],
                VerdictType.TimeLimitExceeded => _localizer["TimeLimitExceeded"],
                VerdictType.MemoryLimitExceeded => _localizer["MemoryLimitExceeded"],
                VerdictType.UnexpectedError => _localizer["UnexpectedError"],
                VerdictType.PartialSolution => _localizer["PartialSolution"],
                VerdictType.Accepted => _localizer["Accepted"],
                VerdictType.TestInProgress => _localizer["TestInProgress"],
                VerdictType.Undefined => "Undefined",
                _ => "",
            };

            List<TestResultBaseModel> loadedTestResults = await _dbContext.TestsResults.Where(tr => tr.SolutionId == baseModel.Id)
                                                                                        .ToListAsync();

            TestsResults = (List<TestResultEntryOutputModel>)loadedTestResults.ConvertAll(async tr =>
                                                                                            {
                                                                                                TestResultEntryOutputModel trOut = new TestResultEntryOutputModel(_localizer);
                                                                                                await trOut.TransformForOutputAsync(tr);
                                                                                                return trOut;
                                                                                            })
                                                                                .Select(trOut => trOut.Result);

        }
    }
}