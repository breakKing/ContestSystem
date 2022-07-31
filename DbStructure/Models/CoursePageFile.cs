namespace ContestSystem.DbStructure.Models
{
    public class CoursePageFile: BaseFile
    {
        public long CoursePageId { get; set; }
        public virtual CoursePage CoursePage { get; set; }
    }
}
