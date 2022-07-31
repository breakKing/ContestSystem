using ContestSystem.DbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class RulesSetRequestForm
    {
        [Required] public long RulesSetId { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
