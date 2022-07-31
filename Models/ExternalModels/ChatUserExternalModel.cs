using ContestSystem.DbStructure.Models.Messenger;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatUserExternalModel
    {
        public long ChatId { get; set; }
        public string ChatLink { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }

        public static ChatUserExternalModel GetFromModel(ChatUser chatUser, string name)
        {
            if (chatUser == null)
            {
                return null;
            }

            return new ChatUserExternalModel
            {
                ChatId = chatUser.ChatId,
                ChatLink = chatUser.Chat.Link,
                Id = chatUser.UserId,
                Name = name
            };
        }
    }
}
