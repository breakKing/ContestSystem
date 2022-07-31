using ContestSystem.DbStructure.Models.Auth;
using System;

namespace ContestSystem.DbStructure.Models.Messenger
{
    public abstract class BaseMessage<TMessage>: BaseEntity
        where TMessage: class
    {
        public string Text { get; set; }
        public long? SenderId { get; set; }
        public virtual User Sender { get; set; }
        public DateTime SentDateTimeUTC { get; set; }
        public DateTime UpdateDateTimeUTC { get; set; }
    }
}
