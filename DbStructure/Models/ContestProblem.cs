namespace ContestSystem.DbStructure.Models
{
    public class ContestProblem: BaseEntityWithoutId
    {
        public long ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        public long ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public char Letter { get; set; }
    }
}
