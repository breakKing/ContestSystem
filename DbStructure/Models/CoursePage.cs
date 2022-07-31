using System.Collections.Generic;

namespace ContestSystem.DbStructure.Models
{
    public class CoursePage: BaseEntity
    {
        public long CourseId { get; set; }
        public virtual Course Course { get; set; }
        public long? CoursePageParentId { get; set; }
        public virtual CoursePage CoursePageParent { get; set; }
        public string HtmlText { get; set; }

        public virtual List<CoursePageFile> CoursePageFiles { get; set; }
    }
}
