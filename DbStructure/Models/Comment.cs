using ContestSystem.DbStructure.Models.Auth;
using System;

namespace ContestSystem.DbStructure.Models
{
    public class Comment: BaseEntity
    {
        public long PostId { get; set; }
        public virtual Post Post { get; set; }
        public string Text { get; set; }
        public long? CommentToReplyId { get; set; }
        public virtual Comment CommentToReply { get; set; }
        public long? SenderId { get; set; }
        public virtual User Sender { get; set; }
        public DateTime SentDateTimeUTC { get; set; }
    }
}
