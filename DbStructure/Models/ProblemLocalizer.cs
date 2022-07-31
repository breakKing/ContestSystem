
using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class ProblemLocalizer : BaseLocalizer
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputBlock { get; set; }
        public string OutputBlock { get; set; }
        public long ProblemId { get; set; }
        [JsonIgnore] public virtual Problem Problem { get; set; }
    }
}