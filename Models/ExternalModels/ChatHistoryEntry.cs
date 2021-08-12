using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models.Messenger;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatHistoryEntry
    {
        public long Id { get; set; }
        public string ChatLink { get; set; }
        public long? UserId { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public ChatEventType Type { get; set; } // Имеет значение, если эта "entry" - НЕ сообщение, иначе ChatEventType.Undefined
        public string Text { get; set; } // Имеет значение, если эта "entry" - сообщение, иначе null
        public ChatHistoryEntry MessageToReply { get; set; } // Имеет значение, если эта "entry" - сообщение, и при этом оно является ответом, иначе null

        public static ChatHistoryEntry GetFromModel(ChatEvent chatEvent)
        {
            return new ChatHistoryEntry
            {
                Id = chatEvent.Id,
                ChatLink = chatEvent.Chat?.Link,
                UserId = chatEvent.UserId,
                DateTimeUTC = chatEvent.DateTimeUTC,
                Type = chatEvent.Type,
                Text = null,
                MessageToReply = null
            };
        }

        public static ChatHistoryEntry GetFromModel(ChatMessage chatMessage, bool stopRecursion = false)
        {
            return new ChatHistoryEntry
            {
                Id = chatMessage.Id,
                ChatLink = chatMessage.Chat?.Link,
                UserId = chatMessage.SenderId,
                DateTimeUTC = chatMessage.SentDateTimeUTC,
                Type = ChatEventType.Undefined,
                Text = chatMessage.Text,
                MessageToReply = stopRecursion ? null : GetFromModel(chatMessage.MessageToReply, true)
            };
        }
    }
}
