using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PostBaseInfo
    {
        public long Id { get; set; }
        public object Author { get; set; }
        public string LocalizedName { get; set; }
        public string PreviewImage { get; set; }
        public string PreviewText { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public ApproveType ApprovalStatus { get; set; }

        public static PostBaseInfo GetFromModel(Post post, PostLocalizer localizer, string imageInBase64)
        {
            if (post == null)
            {
                return null;
            }

            return new PostBaseInfo
            {
                Id = post.Id,
                LocalizedName = localizer?.Name,
                PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                Author = post.Author?.ResponseStructure,
                PreviewImage = imageInBase64,
                PreviewText = localizer?.PreviewText,
                ApprovalStatus = post.ApprovalStatus
            };
        }
    }
}
