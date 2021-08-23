using ContestSystemDbStructure.Models.Messenger;

namespace ContestSystem.Models.ExternalModels
{
    public class ChatUserExternalModel
    {
        public string ChatLink { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }

        public static ChatUserExternalModel GetFromModel(ChatUser chatUser, string name)
        {
            if (chatUser == null)
            {
                return null;
            }

            return new ChatUserExternalModel
            {
                ChatLink = chatUser.Chat.Link,
                UserId = chatUser.UserId,
                Name = name
            };
        }
    }
}
