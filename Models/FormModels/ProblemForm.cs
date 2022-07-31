using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.FormModels
{
    public class ProblemForm
    {
        public long? Id { get; set; }
        [Required] public long CreatorId { get; set; }
        [Required] public List<ProblemLocalizerForm> Localizers { get; set; }
        [Required] public long MemoryLimitInBytes { get; set; }
        [Required] public int TimeLimitInMilliseconds { get; set; }
        [Required] public bool IsPublic { get; set; }
        [Required] public long CheckerId { get; set; }
        [Required] public List<TestForm> Tests { get; set; }
        [Required] public List<ExampleForm> Examples { get; set; }
    }
}
