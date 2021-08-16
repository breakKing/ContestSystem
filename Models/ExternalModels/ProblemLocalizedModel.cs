using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ProblemLocalizedModel
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public string LocalizedInputBlock { get; set; }
        public string LocalizedOutputBlock { get; set; }
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public List<Example> Examples { get; set; } = new List<Example>();

        public static ProblemLocalizedModel GetFromModel(Problem problem, ProblemLocalizer localizer)
        {
            if (problem == null)
            {
                return null;
            }

            return new ProblemLocalizedModel
            {
                Id = problem.Id,
                LocalizedName = localizer?.Name,
                LocalizedDescription = localizer?.Description,
                LocalizedInputBlock = localizer?.InputBlock,
                LocalizedOutputBlock = localizer?.OutputBlock,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                Examples = problem.Examples
            };
        }
    }
}