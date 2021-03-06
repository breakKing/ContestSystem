using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class MessageBaseModel
    {
        public long Id { get; set; }
        public long ContestId { get; set; }
        public ContestBaseModel Contest { get; set; }
        public string Text { get; set; }
        public long? MessageToReplyId { get; set; }
        public MessageBaseModel MessageToReply { get; set; }
        public string SenderId { get; set; }
        public UserBaseModel Sender { get; set; }
        public bool IsPublic { get; set; }
    }
}
