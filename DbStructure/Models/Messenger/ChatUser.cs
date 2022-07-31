using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models.Messenger
{
    public class ChatUser : BaseEntityWithoutId
    {
        public long ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public bool ConfirmedByChatAdmin { get; set; }
        public bool ConfirmedByThemselves { get; set; }
        public bool MutedChat { get; set; }
    }
}
