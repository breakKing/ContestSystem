using ContestSystem.DbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class PostRequestForm
    {
        [Required] public long PostId { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
