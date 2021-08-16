using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ParticipantExternalModel
    {
        public long ContestId { get; set; }
        public long UserId { get; set; }
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
                UserId = contestParticipant.ParticipantId,
                Alias = contestParticipant.Alias
            };
        }
    }
}
