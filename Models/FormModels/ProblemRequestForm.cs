using ContestSystem.DbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ProblemRequestForm
    {
        [Required] public long ProblemId { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
