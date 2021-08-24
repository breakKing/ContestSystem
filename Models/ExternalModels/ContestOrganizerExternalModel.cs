using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestOrganizerExternalModel
    {
        public long ContestId { get; set; }
        public long Id { get; set; }
        public string Alias { get; set; }

        public static ContestOrganizerExternalModel GetFromModel(ContestLocalModerator localModerator)
        {
            if (localModerator == null)
            {
                return null;
            }

            return new ContestOrganizerExternalModel
            {
                ContestId = localModerator.ContestId,
                Id = localModerator.LocalModeratorId,
                Alias = localModerator.Alias
            };
        }
    }
}
