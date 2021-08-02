using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ChatMessageForm
    {
        [Required] public string ChatLink { get; set; }
        [Required] public string Text { get; set; }
        [Required] public long UserId { get; set; }
    }
}
