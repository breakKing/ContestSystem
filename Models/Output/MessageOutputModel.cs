using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class MessageOutputModel : IOutputModel<MessageBaseModel>
    {
        public string Text { get; set; }
        public string SenderUsername { get; set; }
        public DateTime SentDateTimeUTC { get; set; }
        public string MessageToReplyText { get; set; }
        public string MessageToReplySenderUsername { get; set; }
        public DateTime MessageToReplySentDateTimeUTC { get; set; }

        public void TransformForOutput(MessageBaseModel baseModel)
        {
            Text = baseModel.Text;
            SenderUsername = baseModel.Sender.NormalizedUserName;
            SentDateTimeUTC = baseModel.SentDateTimeUTC;
            if (baseModel.MessageToReplyId != null)
            {
                MessageToReplyText = baseModel.MessageToReply.Text;
                MessageToReplySenderUsername = baseModel.MessageToReply.Sender.NormalizedUserName;
                MessageToReplySentDateTimeUTC = baseModel.MessageToReply.SentDateTimeUTC;
            }
        }

        public async Task TransformForOutputAsync(MessageBaseModel baseModel)
        {
            TransformForOutput(baseModel);

        }
    }
}