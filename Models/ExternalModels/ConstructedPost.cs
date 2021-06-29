using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedPost
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

        public static ConstructedPost GetFromModel(Post post, string imageInBase64)
        {
            return new ConstructedPost
            {
                Id = post.Id,
                Author = post.Author?.ResponseStructure,
                PromotedDateTimeUTC = post.PromotedDateTimeUTC,
                ApprovalStatus = post.ApprovalStatus,
                ApprovingModerator = post.ApprovingModerator?.ResponseStructure,
                Localizers = post.PostLocalizers?.ConvertAll(pl => PostLocalizerExternalModel.GetFromModel(pl)),
                ModerationMessage = post.ModerationMessage,
                PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                PreviewImage = imageInBase64
            };
        }
    }
}
