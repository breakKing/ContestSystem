using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ExampleForm
    {
        [Required] public short Number { get; set; }
        [Required] public string InputText { get; set; }
        [Required] public string OutputText { get; set; }
        [Required] public long ProblemId { get; set; }
    }
}