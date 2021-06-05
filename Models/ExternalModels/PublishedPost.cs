using ContestSystemDbStructure.Enums;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedPost
    {
        public long Id { get; set; }
        public object Author { get; set; }
        public string LocalizedName { get; set; }
        public string PreviewImage { get; set; }
        public string PreviewText { get; set; }
        public string HtmlLocalizedText { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public string ModerationMessage { get; set; }
        public ApproveType ApprovalStatus { get; set; }
    }
}
