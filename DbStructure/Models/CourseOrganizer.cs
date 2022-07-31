using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class CourseOrganizer: BaseEntityWithoutId
    {
        public long CourseId { get; set; }
        public virtual Course Course { get; set; }
        public long OrganizerId { get; set; }
        public virtual User Organizer { get; set; }
        public string Alias { get; set; }
    }
}
