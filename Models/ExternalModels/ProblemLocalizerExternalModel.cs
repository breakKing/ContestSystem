using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ProblemLocalizerExternalModel
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputBlock { get; set; }
        public string OutputBlock { get; set; }
        public long ProblemId { get; set; }

        public static ProblemLocalizerExternalModel GetFromModel(ProblemLocalizer localizer)
        {
            if (localizer == null)
            {
                return null;
            }

            return new ProblemLocalizerExternalModel
            {
                Culture = localizer.Culture,
                Name = localizer.Name,
                Description = localizer.Description,
                InputBlock = localizer.InputBlock,
                OutputBlock = localizer.OutputBlock,
                ProblemId = localizer.ProblemId
            };
        }
    }
}
