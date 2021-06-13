using ContestSystemDbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ContestRequestForm
    {
        [Required] public long ContestId { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
