using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models.Messenger;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatExternalModel
    {
        public long Id { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long AdminId { get; set; }
        public ChatType Type { get; set; }
        public long? ContestId { get; set; }
        public List<ChatUserExternalModel> Users { get; set; }
        public List<ChatHistoryEntry> HistoryEntries { get; set; } = new List<ChatHistoryEntry>();

        public static ChatExternalModel GetFromModel(Chat chat, List<ChatUserExternalModel> users, List<ChatHistoryEntry> historyEntries, string imageInBase64)
        {
            if (chat == null)
            {
                return null;
            }

            return new ChatExternalModel
            {
                Id = chat.Id,
                ContestId = chat.ContestId,
                Link = chat.Link,
                Name = chat.Name,
                Image = imageInBase64,
                AdminId = chat.AdminId.GetValueOrDefault(),
                Type = chat.Type,
                Users = users,
                HistoryEntries = historyEntries
            };
        }
    }
}
