using System.ComponentModel.DataAnnotations;

namespace ContestSystem.DbStructure.Models
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
