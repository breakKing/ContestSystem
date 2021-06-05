using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ContestSystem.Models.FormModels
{
    public class PostForm
    {
        public long? Id { get; set; }
        [Required] public long AuthorUserId { get; set; }
        public IFormFile PreviewImage { get; set; }
        [Required] public List<PostLocalizerForm> Localizers { get; set; }
    }
}