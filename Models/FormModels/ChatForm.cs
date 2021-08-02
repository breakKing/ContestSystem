using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ChatForm
    {
        public string Link { get; set; } // Ссылка чата всегда уникальна, поэтому используется вместо Id
        [Required] public string Name { get; set; }
        [Required] public long AdminId { get; set; }
        [Required] public bool AnyoneCanJoin { get; set; }
        public IFormFile Image { get; set; }
        public List<long> InitialUsers { get; set; } // Использовать только при создании чата
    }
}
