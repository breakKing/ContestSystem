namespace ContestSystem.DbStructure.Models
{
    public class CourseLocalizer: BaseLocalizer
    {
        public long CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
