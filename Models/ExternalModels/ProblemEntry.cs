namespace ContestSystem.Models.ExternalModels
{
    public class ProblemEntry
    {
        public long ContestId { get; set; }
        public long ProblemId { get; set; }
        public char Letter { get; set; }
        public ProblemLocalizedModel Problem { get; set; }
    }
}