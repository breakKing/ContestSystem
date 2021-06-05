using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ProblemEntryForm
    {
        [Required] public long ProblemId { get; set; }
        [Required] public char Letter { get; set; }
    }
}