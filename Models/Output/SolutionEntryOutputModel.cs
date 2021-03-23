using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class SolutionEntryOutputModel : IOutputModel<SolutionBaseModel>
    {
        private readonly IStringLocalizer<SolutionOutputModel> _localizer;
        private readonly ContestSystemDbContext _dbContext;

        public string Alias { get; set; }
        public string ProblemName { get; set; }
        public string Compiler { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string Verdict { get; set; }
        public short Points { get; set; }

        public SolutionEntryOutputModel(IStringLocalizer<SolutionOutputModel> localizer, ContestSystemDbContext dbContext)
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
            Points = baseModel.Points;
        }

        public async Task TransformForOutputAsync(SolutionBaseModel baseModel)
        {
            ContestProblemBaseModel contestProblem = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ContestId == baseModel.ContestId
                                                                                                                    && cp.ProblemId == baseModel.ProblemId);

            Alias = contestProblem.Alias;
            ProblemName = baseModel.Problem.Name;
            Compiler = baseModel.Compiler;
            SubmitTimeUTC = baseModel.SubmitTimeUTC;
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
            Points = baseModel.Points;
        }
    }
}
