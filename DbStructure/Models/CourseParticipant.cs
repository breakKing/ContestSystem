using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class CourseParticipant: BaseEntityWithoutId
    {
        public long CourseId { get; set; }
        public virtual Course Course { get; set; }
        public long ParticipantId { get; set; }
        public virtual User Participant { get; set; }
        public bool ConfirmedByOrganizer { get; set; }
        public bool ConfirmedByParticipant { get; set; }
        public long? ConfirmingOrganizerId { get; set; }
        public virtual User ConfirmingOrganizer { get; set; }
        public string Alias { get; set; }
    }
}
