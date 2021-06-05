using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class PostRequestForm
    {
        [Required] public long Id { get; set; }
        [Required] public User Author { get; set; }
        [Required] public List<PostLocalizerForm> Localizers { get; set; } = new List<PostLocalizerForm>();
        [Required] public DateTime PromotedDateTimeUTC { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
