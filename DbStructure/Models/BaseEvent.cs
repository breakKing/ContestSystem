using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class BaseEvent: BaseEntity
    {
        public bool IsPublic { get; set; }
        public long? CreatorId { get; set; }
        public virtual User Creator { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public long? ApprovingModeratorId { get; set; }
        public virtual User ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
    }
}
