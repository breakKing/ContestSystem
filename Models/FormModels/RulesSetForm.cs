using ContestSystem.DbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class RulesSetForm
    {
        public long? Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        [Required] public bool IsPublic { get; set; }
        [Required] public long AuthorId { get; set; }
        [Required] public RulesCountMode CountMode { get; set; }
        [Required] public bool PenaltyForCompilationError { get; set; }
        [Required] public long PenaltyForOneTry { get; set; }
        [Required] public long PenaltyForOneMinute { get; set; }
        [Required] public bool PointsForBestSolution { get; set; }
        [Required] public int MaxTriesForOneProblem { get; set; }
        [Required] public bool PublicMonitor { get; set; }
        [Required] public short MonitorFreezeTimeBeforeFinishInMinutes { get; set; }
        [Required] public bool ShowFullTestsResults { get; set; }
    }
}
