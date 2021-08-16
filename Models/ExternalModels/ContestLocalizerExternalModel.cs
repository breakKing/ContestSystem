using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestLocalizerExternalModel
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ContestId { get; set; }

        public static ContestLocalizerExternalModel GetFromModel(ContestLocalizer localizer)
        {
            if (localizer == null)
            {
                return null;
            }

            return new ContestLocalizerExternalModel
            {
                Culture = localizer.Culture,
                Name = localizer.Name,
                Description = localizer.Description,
                ContestId = localizer.ContestId
            };
        }
    }
}
