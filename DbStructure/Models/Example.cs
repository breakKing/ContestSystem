using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class Example : BaseEntityWithoutId
    {
        public short Number { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public long ProblemId { get; set; }
        [JsonIgnore] public virtual Problem Problem { get; set; }
    }
}