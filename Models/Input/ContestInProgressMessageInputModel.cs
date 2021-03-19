using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class ContestInProgressMessageInputModel : IInputModel<MessageBaseModel, long?>
    {
        public long? Id { get; set; }
        public long ContestId { get; set; }
        public string Text { get; set; }
        public long? MessageToReplyId { get; set; }
        public string SenderId { get; set; }
        public bool IsPublic { get; set; }

        public MessageBaseModel ReadFromInput()
        {
            return new MessageBaseModel
            {
                Id = Id.GetValueOrDefault(),
                SubjectId = ContestId,
                SenderId = SenderId,
                Text = Text,
                MessageToReplyId = MessageToReplyId,
                Type = MessageType.ChatWhileContestInProgress,
                IsPublic = IsPublic,
                SentDateTimeUTC = DateTime.UtcNow
            };
        }

        public async Task<MessageBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
