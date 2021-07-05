using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedProblem
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public string LocalizedInputBlock { get; set; }
        public string LocalizedOutputBlock { get; set; }
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public string ModerationMessage { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public List<Example> Examples { get; set; } = new List<Example>();

        public static PublishedProblem GetFromModel(Problem problem, ProblemLocalizer localizer)
        {
            return new PublishedProblem
            {
                Id = problem.Id,
                LocalizedName = localizer?.Name,
                LocalizedDescription = localizer?.Description,
                LocalizedInputBlock = localizer?.InputBlock,
                LocalizedOutputBlock = localizer?.OutputBlock,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                ModerationMessage = problem.ModerationMessage,
                ApprovalStatus = problem.ApprovalStatus,
                IsPublic = problem.IsPublic,
                IsArchieved = problem.IsArchieved,
                Examples = problem.Examples
            };
        }
    }
}