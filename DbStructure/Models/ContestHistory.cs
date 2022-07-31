using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class ContestHistory: BaseEntity
    {
        public int SecondsAfterStart { get; set; }
        public long ParticipantId { get; set; }
        public virtual User Participant { get; set; }
        public long ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        public long ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public VerdictType Verdict { get; set; }
        public long AddedResult { get; set; }
    }
}
