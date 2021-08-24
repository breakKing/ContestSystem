using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models.Messenger;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatHistoryEntry
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public string ChatLink { get; set; }
        public ChatUserExternalModel AffectedUser { get; set; }
        public ChatUserExternalModel Initiator { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public ChatEventType Type { get; set; } // Имеет значение, если эта "entry" - НЕ сообщение, иначе ChatEventType.Undefined
        public string Text { get; set; } // Имеет значение, если эта "entry" - сообщение, иначе null

        public static ChatHistoryEntry GetFromModel(ChatEvent chatEvent, ChatUserExternalModel affectedUser, 
            ChatUserExternalModel initiator)
        {
            if (chatEvent == null)
            {
                return null;
            }

            return new ChatHistoryEntry
            {
                Id = chatEvent.Id,
                ChatId = chatEvent.ChatId,
                ChatLink = chatEvent.Chat?.Link,
                Initiator = affectedUser,
                AffectedUser = initiator,
                DateTimeUTC = chatEvent.DateTimeUTC,
                Type = chatEvent.Type,
                Text = null
            };
        }

        public static ChatHistoryEntry GetFromModel(ChatMessage chatMessage, ChatUserExternalModel sender)
        {
            if (chatMessage == null)
            {
                return null;
            }

            return new ChatHistoryEntry
            {
                Id = chatMessage.Id,
                ChatId = chatMessage.ChatId,
                ChatLink = chatMessage.Chat?.Link,
                Initiator = sender,
                AffectedUser = null,
                DateTimeUTC = chatMessage.SentDateTimeUTC,
                Type = ChatEventType.Undefined,
                Text = chatMessage.Text
            };
        }
    }
}
