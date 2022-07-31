using System.ComponentModel.DataAnnotations;

namespace ContestSystem.DbStructure.Models
{
    public class BaseEntityWithoutId
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
