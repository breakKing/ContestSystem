using ContestSystemDbStructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatExternalModel
    {
        public string Link { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long AdminId { get; set; }
        public List<object> Users { get; set; }
        public List<ChatEventExternalModel> Events { get; set; }
        public List<ChatMessageExternalModel> Messages { get; set; }

        public static ChatExternalModel GetFromModel(Chat chat, List<ChatUser> users, List<Message> messages, List<ChatEvent> events, string imageInBase64)
        {
            return new ChatExternalModel
            {
                Link = chat.Link,
                Name = chat.Name,
                Image = imageInBase64,
                AdminId = chat.AdminId.GetValueOrDefault(),
                Users = users.ConvertAll(u => u.User?.ResponseStructure),
                Events = events.ConvertAll(e => ChatEventExternalModel.GetFromModel(chat, e))
                                .OrderByDescending(e => e.DateTimeUTC)
                                .ToList(),
                Messages = messages.ConvertAll(m => ChatMessageExternalModel.GetFromModel(chat, m, m.MessageToReply))
                                .OrderByDescending(m => m.DateTimeUTC)
                                .ToList()
            };
        }
    }
}
