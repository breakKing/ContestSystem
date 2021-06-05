using ContestSystemDbStructure.Enums;

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
    }
}
