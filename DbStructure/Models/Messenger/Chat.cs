using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models.Auth;
using System.Collections.Generic;

namespace ContestSystem.DbStructure.Models.Messenger
{
    public class Chat: BaseEntity
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string ImagePath { get; set; }
        public bool AnyoneCanJoin { get; set; }
        public bool IsCreatedBySystem { get; set; }
        public ChatType Type { get; set; }
        public long? AdminId { get; set; }
        public virtual User Admin { get; set; }
        public long? ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        
        public virtual List<ChatEvent> ChatEvents { get; set; }
        public virtual List<ChatMessage> ChatMessages { get; set; }

        public virtual List<ChatUser> ChatUsers { get; set; }
        public virtual List<User> Users { get; set; } 
    }
}
