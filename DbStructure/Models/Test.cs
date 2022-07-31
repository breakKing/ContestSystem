using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class Test : BaseEntityWithoutId
    {
        public short Number { get; set; }
        public long ProblemId { get; set; }
        [JsonIgnore] public virtual Problem Problem { get; set; }
        public string Input { get; set; }
        public string Answer { get; set; }
        public short AvailablePoints { get; set; }
    }
}