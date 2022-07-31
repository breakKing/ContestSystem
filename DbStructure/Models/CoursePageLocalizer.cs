namespace ContestSystem.DbStructure.Models
{
    public class CoursePageLocalizer : BaseLocalizer
    {
        public long CoursePageId { get; set; }
        public virtual CoursePage CoursePage { get; set; }
        public string Title { get; set; }
        public string HtmlText { get; set; }
    }
}
