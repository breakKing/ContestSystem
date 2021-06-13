using ContestSystemDbStructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class CourseRequestForm
    {
        [Required] public long CourseId { get; set; }
        [Required] public ApproveType ApprovalStatus { get; set; }
        [Required] public long ApprovingModeratorId { get; set; }
        public string ModerationMessage { get; set; }
    }
}
