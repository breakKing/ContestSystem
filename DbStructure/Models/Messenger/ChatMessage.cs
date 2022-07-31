namespace ContestSystem.DbStructure.Models.Messenger
{
    public class ChatMessage: BaseMessage<ChatMessage>
    {
        public long ChatId { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
