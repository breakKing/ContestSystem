using ContestSystemDbStructure.Enums;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedChecker
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public bool IsPublic { get; set; }
        public VerdictType CompilationVerdict { get; set; }
        public string Errors { get; set; }
        public string ModerationMessage { get; set; }
        public ApproveType ApprovalStatus { get; set; }
    }
}
