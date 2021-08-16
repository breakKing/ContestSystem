using ContestSystemDbStructure.Models.Messenger;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PrivateMessageExternalModel
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public PrivateMessageExternalModel MessageToReply { get; set; }

        public static PrivateMessageExternalModel GetFromModel(PrivateMessage message, PrivateMessage messageToReply, bool stopRecursion = false)
        {
            if (message == null)
            {
                return null;
            }

            return new PrivateMessageExternalModel
            {
                Id = message.Id,
                SenderId = message.SenderId.GetValueOrDefault(-1),
                ReceiverId = message.ReceiverId.GetValueOrDefault(-1),
                Text = message.Text,
                DateTimeUTC = message.SentDateTimeUTC,
                MessageToReply = stopRecursion ? null : GetFromModel(messageToReply, null, true)
            };
        }
    }
}
