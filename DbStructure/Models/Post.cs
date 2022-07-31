using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using System;
using System.Collections.Generic;

namespace ContestSystem.DbStructure.Models
{
    public class Post: BaseEntity
    {
        public long? AuthorId { get; set; }
        public virtual User Author { get; set; }
        public string ImagePath { get; set; }
        public DateTime PromotedDateTimeUTC { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public long? ApprovingModeratorId { get; set; }
        public virtual User ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public virtual List<PostLocalizer> PostLocalizers { get; set; } = new List<PostLocalizer>();
    }
}
