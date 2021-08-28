using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestOrganizerExternalModel
    {
        public long ContestId { get; set; }
        public long Id { get; set; }
        public string Alias { get; set; }

        public static ContestOrganizerExternalModel GetFromModel(ContestOrganizer organizer)
        {
            if (organizer == null)
            {
                return null;
            }

            return new ContestOrganizerExternalModel
            {
                ContestId = organizer.ContestId,
                Id = organizer.OrganizerId,
                Alias = organizer.Alias
            };
        }
    }
}
