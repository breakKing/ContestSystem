using ContestSystemDbStructure.Models.Messenger;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatMessageExternalModel
    {
        public long Id { get; set; }
        public string ChatLink { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public ChatMessageExternalModel MessageToReply { get; set; }

        public static ChatMessageExternalModel GetFromModel(Chat chat, ChatMessage message, ChatMessage messageToReply, bool stopRecursion = false)
        {
            return new ChatMessageExternalModel
            {
                Id = message.Id,
                ChatLink = chat.Link,
                UserId = message.SenderId.GetValueOrDefault(-1),
                Text = message.Text,
                DateTimeUTC = message.SentDateTimeUTC,
                MessageToReply = stopRecursion ? null : GetFromModel(chat, messageToReply, null, true)
            };
        }
    }
}
