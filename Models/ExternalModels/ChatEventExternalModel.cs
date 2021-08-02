using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatEventExternalModel
    {
        public ulong Id { get; set; }
        public string ChatLink { get; set; }
        public ChatEventType Type { get; set; }
        public long? UserId { get; set; }
        public DateTime DateTimeUTC { get; set; }

        public static ChatEventExternalModel GetFromModel(Chat chat, ChatEvent chatEvent)
        {
            return new ChatEventExternalModel
            {
                Id = chatEvent.Id,
                ChatLink = chat.Link,
                Type = chatEvent.Type,
                UserId = chatEvent.UserId,
                DateTimeUTC = chatEvent.DateTimeUTC
            };
        }
    }
}