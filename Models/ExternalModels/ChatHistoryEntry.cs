using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models.Messenger;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatHistoryEntry
    {
        public long Id { get; set; }
        public string ChatLink { get; set; }
        public object AffectedUser { get; set; }
        public object Initiator { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public ChatEventType Type { get; set; } // Имеет значение, если эта "entry" - НЕ сообщение, иначе ChatEventType.Undefined
        public string Text { get; set; } // Имеет значение, если эта "entry" - сообщение, иначе null

        public static ChatHistoryEntry GetFromModel(ChatEvent chatEvent)
        {
            if (chatEvent == null)
            {
                return null;
            }

            return new ChatHistoryEntry
            {
                Id = chatEvent.Id,
                ChatLink = chatEvent.Chat?.Link,
                Initiator = chatEvent.Initiator?.ResponseStructure,
                AffectedUser = chatEvent.AffectedUser?.ResponseStructure,
                DateTimeUTC = chatEvent.DateTimeUTC,
                Type = chatEvent.Type,
                Text = null
            };
        }

        public static ChatHistoryEntry GetFromModel(ChatMessage chatMessage)
        {
            if (chatMessage == null)
            {
                return null;
            }

            return new ChatHistoryEntry
            {
                Id = chatMessage.Id,
                ChatLink = chatMessage.Chat?.Link,
                Initiator = chatMessage.Sender?.ResponseStructure,
                AffectedUser = null,
                DateTimeUTC = chatMessage.SentDateTimeUTC,
                Type = ChatEventType.Undefined,
                Text = chatMessage.Text
            };
        }
    }
}
