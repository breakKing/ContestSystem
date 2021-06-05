using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class TestForm
    {
        [Required] public short Number { get; set; }
        [Required] public long ProblemId { get; set; }
        [Required] public string Input { get; set; }
        [Required] public string Answer { get; set; }
        [Required] public short AvailablePoints { get; set; }
    }
}