using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class MonitorEntry
    {
        public long ContestId { get; set; }
        public long UserId { get; set; }
        public string Alias { get; set; }
        public int Position { get; set; }
        public long Result { get; set; } // В зависимости от RulesSet.CountMode: Points (1) - очки, Penalty (2) - штраф, PointsMinusPenalty (3) - счёт
        public short ProblemsSolvedCount { get; set; } // используется только при RulesSet.CountMode = Penalty (2)
        public List<ProblemTriesEntry> ProblemTries { get; set; } = new List<ProblemTriesEntry>();
    }
}
