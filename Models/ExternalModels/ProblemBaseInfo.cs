using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ProblemBaseInfo
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }

        public static ProblemBaseInfo GetFromModel(Problem problem, ProblemLocalizer localizer)
        {
            if (problem == null)
            {
                return null;
            }

            return new ProblemBaseInfo
            {
                Id = problem.Id,
                LocalizedName = localizer?.Name,
                LocalizedDescription = localizer?.Description,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                ApprovalStatus = problem.ApprovalStatus,
                IsArchieved = problem.IsArchieved,
                IsPublic = problem.IsPublic
            };
        }
    }
}
