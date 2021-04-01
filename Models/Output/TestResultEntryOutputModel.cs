using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class TestResultEntryOutputModel : IOutputModel<TestResultBaseModel>
    {
        private readonly IStringLocalizer<SolutionOutputModel> _localizer;

        public short Number { get; set; }
        public long UsedMemoryInKilobytes { get; set; }
        public double UsedTimeInSeconds { get; set; }
        public short GotPoints { get; set; }
        public string Verdict { get; set; }

        public TestResultEntryOutputModel(IStringLocalizer<SolutionOutputModel> localizer)
        {
            _localizer = localizer;
        }

        public void TransformForOutput(TestResultBaseModel baseModel)
        {
            Number = baseModel.Number;
            UsedMemoryInKilobytes = baseModel.UsedMemoryInBytes / 1024;
            UsedTimeInSeconds = baseModel.UsedTimeInMillis / 1000.0;
            GotPoints = baseModel.GotPoints;
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

        public async Task TransformForOutputAsync(TestResultBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
