using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ProblemLocalizerForm
    {
        [Required] public string Culture { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string InputBlock { get; set; }
        [Required] public string OutputBlock { get; set; }
    }
}