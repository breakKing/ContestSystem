using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class PostLocalizerForm
    {
        [Required] public string Culture { get; set; }
        [Required] public string PreviewText { get; set; }
        [Required] public string HtmlText { get; set; }
        [Required] public string Name { get; set; }
    }
}