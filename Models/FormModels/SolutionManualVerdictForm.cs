using ContestSystem.DbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class SolutionManualVerdictForm
    {
        [Required] public long SolutionId { get; set; }
        [Required] public VerdictType Verdict { get; set; }
        public short Points { get; set; }
    }
}
