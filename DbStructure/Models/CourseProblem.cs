namespace ContestSystem.DbStructure.Models
{
    public class CourseProblem: BaseEntityWithoutId
    {
        public long CourseId { get; set; }
        public virtual Course Course { get; set; }
        public long ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
    }
}
