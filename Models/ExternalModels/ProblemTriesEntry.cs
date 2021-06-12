namespace ContestSystem.Models.ExternalModels
{
    public class ProblemTriesEntry
    {
        public long ContestId { get; set; }
        public long ProblemId { get; set; }
        public long UserId { get; set; }
        public char Letter { get; set; }
        public int TriesCount { get; set; }
        public short GotPoints { get; set; } // Используется, если RulesSet.CountMode != Penalty (2)
        public bool Solved { get; set; } // Используется, если RulesSet.CountMode = Penalty (2)
        public short LastTryMinutesAfterStart { get; set; } // На какой минуте данным юзером было отправлено последнее решение данной задачи
    }
}