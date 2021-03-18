using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class MessageOutputModel : IOutputModel<MessageBaseModel>
    {
        public string Text { get; set; }
        public string SenderUsername { get; set; }
        public string SentDateTime { get; set; }
        public string MessageToReplyText { get; set; }
        public string MessageToReplySenderUsername { get; set; }
        public string MessageToReplySentDateTime { get; set; }

        public void TransformForOutput(MessageBaseModel baseModel)
        {
            Text = baseModel.Text;
            SenderUsername = baseModel.Sender.UserName;
            SentDateTime = baseModel.SentDateTime.ToString("g");
            if (baseModel.MessageToReplyId != null)
            {
                MessageToReplyText = baseModel.MessageToReply.Text;
                MessageToReplySenderUsername = baseModel.MessageToReply.Sender.UserName;
                MessageToReplySentDateTime = baseModel.MessageToReply.SentDateTime.ToString("g");
            }
        }

        public async Task TransformForOutputAsync(MessageBaseModel baseModel)
        {
            TransformForOutput(baseModel);

        }
    }
}