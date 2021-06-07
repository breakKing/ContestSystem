using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
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
        
        public static PublishedPost GetFromModel(Post post, PostLocalizer localizer)
        {
            return new PublishedPost
            {
                Id = post.Id,
                LocalizedName = localizer?.Name,
                HtmlLocalizedText = localizer?.HtmlText,
                PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                Author = post.Author?.ResponseStructure,
                PreviewImage = post.PreviewImage,
                PreviewText = localizer?.PreviewText,
                ApprovalStatus = post.ApprovalStatus
            };
        }
    }
}
