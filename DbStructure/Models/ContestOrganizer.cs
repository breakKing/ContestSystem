using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class ContestOrganizer: BaseEntityWithoutId
    {
        public long ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        public long OrganizerId { get; set; }
        public virtual User Organizer { get; set; }
        public string Alias { get; set; }
    }
}
