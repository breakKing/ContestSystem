using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class FutureContestMessageInputModel : IInputModel<MessageBaseModel, long?>
    {
        public long? Id { get; set; }
        public long ContestId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public long? MessageToReplyId { get; set; }

        public MessageBaseModel ReadFromInput()
        {
            return new MessageBaseModel
            {
                Id = Id.GetValueOrDefault(),
                SubjectId = ContestId,
                SenderId = SenderId,
                Text = Text,
                MessageToReplyId = MessageToReplyId,
                Type = MessageType.ChatWithGlobalContestModerator,
                IsPublic = false
            };
        }

        public async Task<MessageBaseModel> ReadFromInputAsync()
        {
            return new MessageBaseModel
            {
                Id = Id.GetValueOrDefault(),
                SubjectId = ContestId,
                SenderId = SenderId,
                Text = Text,
                MessageToReplyId = MessageToReplyId,
                Type = MessageType.ChatWithGlobalContestModerator,
                IsPublic = false
            };
        }
    }
}
