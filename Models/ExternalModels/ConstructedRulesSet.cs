using ContestSystemDbStructure.Enums;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedRulesSet
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public RulesCountMode CountMode { get; set; }
        public bool PenaltyForCompilationError { get; set; }
        public long PenaltyForOneTry { get; set; }
        public long PenaltyForOneMinute { get; set; }
        public bool PointsForBestSolution { get; set; }
        public int MaxTriesForOneProblem { get; set; }
        public bool PublicMonitor { get; set; }
        public short MonitorFreezeTimeBeforeFinishInMinutes { get; set; }
        public bool ShowFullTestsResults { get; set; }
        public bool IsPublic { get; set; }
    }
}
