using ContestSystem.DbStructure.Models.Auth;
using System.Collections.Generic;

namespace ContestSystem.DbStructure.Models
{
    public class Course: BaseEvent
    {
        public bool IsHidden { get; set; }
        //[Column(TypeName = "varbinary(MAX)")] public byte[] Image { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public virtual List<Solution> Solutions { get; set; }

        public virtual List<CourseProblem> CourseProblems { get; set; }
        public virtual List<Problem> Problems { get; set; }

        public virtual List<CourseParticipant> CourseParticipants { get; set; }
        public virtual List<User> Participants { get; set; }

        public virtual List<CourseOrganizer> CourseOrganizers { get; set; }
        public virtual List<User> Organizers { get; set; }
    }
}
