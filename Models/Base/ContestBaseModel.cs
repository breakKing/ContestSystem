using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ContestBaseModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsForever { get; set; }
        public DateTime StartDateTime { get; set; }
        public uint DurationInMinutes { get; set; }
    }
}
