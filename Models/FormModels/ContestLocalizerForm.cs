using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ContestLocalizerForm
    {
        [Required] public string Culture { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Name { get; set; }
    }
}
