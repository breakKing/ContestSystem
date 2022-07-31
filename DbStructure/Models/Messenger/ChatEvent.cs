using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using System;

namespace ContestSystem.DbStructure.Models.Messenger
{
    public class ChatEvent: BaseEntity
    {
        public long ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        public long? AffectedUserId { get; set; }
        public virtual User AffectedUser { get; set; }
        public long? InitiatorId { get; set; }
        public virtual User Initiator { get; set; }
        public ChatEventType Type { get; set; }
        public DateTime DateTimeUTC { get; set; }
    }
}
