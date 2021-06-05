using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedProblem
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public List<ProblemLocalizer> Localizers { get; set; }
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public string ModerationMessage { get; set; }
        public bool IsPublic { get; set; }
        public Checker Checker { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public List<Test> Tests { get; set; }
        public List<Example> Examples { get; set; }
    }
}
