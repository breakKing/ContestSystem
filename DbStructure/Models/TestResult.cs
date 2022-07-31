using ContestSystem.DbStructure.Enums;
using Newtonsoft.Json;

namespace ContestSystem.DbStructure.Models
{
    public class TestResult : BaseEntityWithoutId
    {
        public short Number { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public long SolutionId { get; set; }
        [JsonIgnore] public virtual Solution Solution { get; set; }
        public long UsedMemoryInBytes { get; set; }
        public int UsedTimeInMillis { get; set; }
        public short GotPoints { get; set; }
        public VerdictType Verdict { get; set; }
        public string CheckerOutput { get; set; }
    }
}