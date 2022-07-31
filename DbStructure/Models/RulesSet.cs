using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using Newtonsoft.Json;

namespace ContestSystem.DbStructure.Models
{
    public class RulesSet : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public long? AuthorId { get; set; }
        [JsonIgnore] public virtual User Author { get; set; }
        public bool IsArchieved { get; set; }
        public RulesCountMode CountMode { get; set; } // Режимы: подсчитывать очки (1), штраф (2) или счёт (3) (очки минус штраф)
        public bool PenaltyForCompilationError { get; set; } // Давать штраф как за неправильное решение в случае ошибки компиляции (работает только при режиме 2)
        public long PenaltyForOneTry { get; set; } // Размер штрафа за одну неверную попытку (одно неверное решение) (работает только при режиме 2)
        public long PenaltyForOneMinute { get; set; } // Размер штрафа за минуту времени (работает при режимах 2 и 3)
        public bool PointsForBestSolution { get; set; } // Засчитывать очки за лучшее решение задачи (true - за лучшее, false - за самое последнее) (работает только в режиме 1)
        public int MaxTriesForOneProblem { get; set; } // Максимальное количество попыток пользователя для одной задачи (работает во всех режимах)
        public bool PublicMonitor { get; set; } // Могут ли видеть монитор люди, не причастные к соревнованию
        public short MonitorFreezeTimeBeforeFinishInMinutes { get; set; } // За сколько минут до конца контеста "замораживается монитор" (результаты в нём перестают обновляться и обновятся лишь в конце контеста)
        public bool ShowFullTestsResults { get; set; } // Показывать ли участникам результаты прогона на каждом тесте (true) или только окончательный вердикт и последний тест (false)
        public ApproveType ApprovalStatus { get; set; }
        public long? ApprovingModeratorId { get; set; }
        public virtual User ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
    }
}