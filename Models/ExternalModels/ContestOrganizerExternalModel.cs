using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestOrganizerExternalModel
    {
        public long ContestId { get; set; }
        public long UserId { get; set; }
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
                UserId = localModerator.LocalModeratorId,
                Alias = localModerator.Alias
            };
        }
    }
}
