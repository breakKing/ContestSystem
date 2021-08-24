using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ParticipantExternalModel
    {
        public long ContestId { get; set; }
        public long Id { get; set; }
        public string Alias { get; set; }

        public static ParticipantExternalModel GetFromModel(ContestParticipant contestParticipant)
        {
            if (contestParticipant == null)
            {
                return null;
            }

            return new ParticipantExternalModel
            {
                ContestId = contestParticipant.ContestId,
                Id = contestParticipant.ParticipantId,
                Alias = contestParticipant.Alias,
            };
        }
    }
}
