using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class Checker: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? AuthorId { get; set; }
        [JsonIgnore] public virtual User Author { get; set; }
        public string Code { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public VerdictType CompilationVerdict { get; set; }
        public string Errors { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public long? ApprovingModeratorId { get; set; }
        [JsonIgnore] public virtual User ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
    }
}
