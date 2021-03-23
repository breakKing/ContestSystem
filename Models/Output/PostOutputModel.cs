using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class PostOutputModel : IOutputModel<PostBaseModel>
    {
        private readonly ContestSystemDbContext _dbContext;

        public string AuthorUsername { get; set; }
        public string Name { get; set; }
        public string HtmlText { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public List<MessageOutputModel> Comments { get; set; } = new List<MessageOutputModel>();

        public PostOutputModel(ContestSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void TransformForOutput(PostBaseModel baseModel)
        {
            AuthorUsername = baseModel.Author.NormalizedUserName;
            Name = baseModel.Name;
            HtmlText = baseModel.HtmlText;
            PublicationDateTimeUTC = baseModel.PublicationDateTimeUTC;

            List<MessageBaseModel> comments = _dbContext.Messages.Where(m => m.Type == MessageType.ChatInPost
                                                                                && m.SubjectId == baseModel.Id
                                                                                && m.MessageToReplyId == null)
                                                                    .OrderBy(m => m.SentDateTimeUTC)
                                                                    .ToList();

            if (comments != null)
            {
                foreach (MessageBaseModel comment in comments)
                {
                    _recursiveSearchForReply(comment, baseModel.Id);
                }
            }
        }

        public async Task TransformForOutputAsync(PostBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }

        private void _recursiveSearchForReply(MessageBaseModel comment, long postId, string tabs = "")
        {
            MessageOutputModel messageOutput = new MessageOutputModel();
            messageOutput.TransformForOutput(comment);
            messageOutput.Text = tabs + messageOutput.Text;
            Comments.Add(messageOutput);

            List<MessageBaseModel> replies = _dbContext.Messages.Where(m => m.Type == MessageType.ChatInPost
                                                                                && m.SubjectId == postId
                                                                                && m.MessageToReplyId == comment.Id)
                                                                .OrderBy(m => m.SentDateTimeUTC)
                                                                .ToList();

            if (replies != null)
            {
                foreach (MessageBaseModel reply in replies)
                {
                    _recursiveSearchForReply(reply, postId, tabs + "\t");
                }
            }
        }

        private async Task _recursiveSearchForReplyAsync(MessageBaseModel comment, long postId)
        {
            MessageOutputModel messageOutput = new MessageOutputModel();
            await messageOutput.TransformForOutputAsync(comment);
            Comments.Add(messageOutput);

            List<MessageBaseModel> replies = await _dbContext.Messages.Where(m => m.Type == MessageType.ChatInPost
                                                                                && m.SubjectId == postId
                                                                                && m.MessageToReplyId == comment.Id)
                                                                .OrderBy(m => m.SentDateTimeUTC)
                                                                .ToListAsync();

            if (replies != null)
            {
                foreach (MessageBaseModel reply in replies)
                {
                    await _recursiveSearchForReplyAsync(reply, postId);
                }
            }
        }
    }
}
