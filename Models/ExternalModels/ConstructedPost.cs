using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedPost
    {
        public long Id { get; set; }
        public object Author { get; set; }
        public List<PostLocalizer> Localizers { get; set; } = new List<PostLocalizer>();
        public string PreviewImage { get; set; }
        public DateTime PromotedDateTimeUTC { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
    }
}
