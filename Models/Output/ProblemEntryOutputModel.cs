using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ProblemEntryOutputModel : IOutputModel<ContestProblemBaseModel>
    {
        private readonly IStringLocalizer<ProblemOutputModel> _localizer;

        public long MemoryLimitInMegabytes { get; set; }
        public double TimeLimitInSeconds { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public ProblemEntryOutputModel(IStringLocalizer<ProblemOutputModel> localizer)
        {
            _localizer = localizer;
        }

        public void TransformForOutput(ContestProblemBaseModel baseModel)
        {
            MemoryLimitInMegabytes = baseModel.Problem.MemoryLimitInBytes / 1024 / 1024;
            TimeLimitInSeconds = baseModel.Problem.TimeLimitInMilliseconds / 1000.0;
            Name = baseModel.Problem.Name;
            Alias = baseModel.Alias;
            Type = baseModel.Problem.Type switch
            {
                ProblemType.FullSolution => _localizer["FullSolution"],
                ProblemType.Scorable => _localizer["Scorable"],
                ProblemType.Undefined => "Undefined",
                _ => "",
            };
        }

        public async Task TransformForOutputAsync(ContestProblemBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
