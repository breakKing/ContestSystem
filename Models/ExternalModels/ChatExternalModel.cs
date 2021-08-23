﻿using ContestSystemDbStructure.Models.Messenger;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatExternalModel
    {
        public string Link { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long AdminId { get; set; }
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
                Link = chat.Link,
                Name = chat.Name,
                Image = imageInBase64,
                AdminId = chat.AdminId.GetValueOrDefault(),
                Users = users,
                HistoryEntries = historyEntries
            };
        }
    }
}
