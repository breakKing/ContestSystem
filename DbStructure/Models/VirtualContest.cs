using ContestSystem.DbStructure.Models.Auth;
using System;

namespace ContestSystem.DbStructure.Models
{
    public class VirtualContest: BaseEntity
    {
        public long ParticipantId { get; set; }
        public virtual User Participant { get; set; }
        public long ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}
