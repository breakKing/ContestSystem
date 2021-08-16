using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class PostLocalizerExternalModel
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string HtmlText { get; set; }
        public string PreviewText { get; set; }
        public long PostId { get; set; }

        public static PostLocalizerExternalModel GetFromModel(PostLocalizer localizer)
        {
            if (localizer == null)
            {
                return null;
            }

            return new PostLocalizerExternalModel
            {
                Culture = localizer.Culture,
                Name = localizer.Name,
                HtmlText = localizer.HtmlText,
                PreviewText = localizer.PreviewText,
                PostId = localizer.PostId
            };
        }
    }
}
