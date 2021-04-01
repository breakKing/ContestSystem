using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class TrainingMonitorEntryOutputModel : IOutputModel<SolutionBaseModel>
    {
        private readonly IStringLocalizer<SolutionOutputModel> _localizer;
        private readonly MainDbContext _dbContext;

        public string ParticipantUsername { get; set; }
        public string ProblemAlias { get; set; }
        public string ProblemName { get; set; }
        public string Compiler { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string Verdict { get; set; }
        public short Points { get; set; }

        public TrainingMonitorEntryOutputModel(IStringLocalizer<SolutionOutputModel> localizer, MainDbContext dbContext)
        {
            _localizer = localizer;
            _dbContext = dbContext;
        }

        public void TransformForOutput(SolutionBaseModel baseModel)
        {
            ParticipantUsername = baseModel.Participant.NormalizedUserName;
            ProblemAlias = _dbContext.ContestsProblems.FirstOrDefault(cp => cp.ContestId == baseModel.ContestId && cp.ProblemId == baseModel.ProblemId).Alias;
            ProblemName = baseModel.Problem.Name;
            Compiler = baseModel.Compiler;
            SubmitTimeUTC = baseModel.SubmitTimeUTC;
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
        }

        public async Task TransformForOutputAsync(SolutionBaseModel baseModel)
        {
            ParticipantUsername = baseModel.Participant.NormalizedUserName;
            ContestProblemBaseModel cp = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ContestId == baseModel.ContestId && cp.ProblemId == baseModel.ProblemId);
            ProblemAlias = cp.Alias;
            ProblemName = baseModel.Problem.Name;
            Compiler = baseModel.Compiler;
            SubmitTimeUTC = baseModel.SubmitTimeUTC;
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
        }
    }
}
