using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models.Messenger
{
    public class PrivateMessage: BaseMessage<PrivateMessage>
    {
        public long? ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
    }
}
