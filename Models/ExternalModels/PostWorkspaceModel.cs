using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class PostWorkspaceModel
    {
        public long Id { get; set; }
        public object Author { get; set; }
        public List<PostLocalizerExternalModel> Localizers { get; set; } = new List<PostLocalizerExternalModel>();
        public string PreviewImage { get; set; }
        public DateTime PromotedDateTimeUTC { get; set; }
        public DateTime PublicationDateTimeUTC { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }

        public static PostWorkspaceModel GetFromModel(Post post, string imageInBase64)
        {
            if (post == null)
            {
                return null;
            }

            return new PostWorkspaceModel
            {
                Id = post.Id,
                Author = post.Author?.ResponseStructure,
                PromotedDateTimeUTC = post.PromotedDateTimeUTC,
                ApprovalStatus = post.ApprovalStatus,
                ApprovingModerator = post.ApprovingModerator?.ResponseStructure,
                Localizers = post.PostLocalizers?.ConvertAll(PostLocalizerExternalModel.GetFromModel),
                ModerationMessage = post.ModerationMessage,
                PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                PreviewImage = imageInBase64
            };
        }
    }
}
