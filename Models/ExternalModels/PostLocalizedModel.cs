using ContestSystemDbStructure.Models;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PostLocalizedModel
    {
        public long Id { get; set; }
        public object Author { get; set; }
        public string LocalizedName { get; set; }
        public string PreviewImage { get; set; }
        public string PreviewText { get; set; }
        public string HtmlLocalizedText { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        
        public static PostLocalizedModel GetFromModel(Post post, PostLocalizer localizer, string imageInBase64)
        {
            if (post == null)
            {
                return null;
            }

            return new PostLocalizedModel
            {
                Id = post.Id,
                LocalizedName = localizer?.Name,
                HtmlLocalizedText = localizer?.HtmlText,
                PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                Author = post.Author?.ResponseStructure,
                PreviewImage = imageInBase64,
                PreviewText = localizer?.PreviewText
            };
        }
    }
}
