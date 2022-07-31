using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class Problem : BaseEntity
    {
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public long? CheckerId { get; set; }
        public virtual Checker Checker { get; set; }
        public long? CreatorId { get; set; }
        [JsonIgnore] public virtual User Creator { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public long? ApprovingModeratorId { get; set; }
        [JsonIgnore] public virtual User ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
        public virtual List<Test> Tests { get; set; }
        public virtual List<Example> Examples { get; set; }
        public virtual List<ProblemLocalizer> ProblemLocalizers { get; set; }

        [JsonIgnore] public virtual List<ContestProblem> ContestProblems { get; set; }
        public virtual List<Contest> Contests { get; set; }

        [JsonIgnore] public virtual List<CourseProblem> CourseProblems { get; set; }
        public virtual List<Course> Courses { get; set; }
    }
}