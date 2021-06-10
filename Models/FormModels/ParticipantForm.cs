using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ParticipantForm
    {
        [Required] public long UserId { get; set; }
        [Required] public long ContestId { get; set; }
        [Required] public string Alias { get; set; }
    }
}
