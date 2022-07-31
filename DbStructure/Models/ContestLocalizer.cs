using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class ContestLocalizer : BaseLocalizer
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ContestId { get; set; }
        [JsonIgnore] public virtual Contest Contest { get; set; }
    }
}