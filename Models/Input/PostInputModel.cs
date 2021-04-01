using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class PostInputModel : IInputModel<Post, long?>
    {
        public long? Id { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
        public string HtmlText { get; set; }

        public Post ReadFromInput()
        {
            return new Post
            {
                Id = Id.GetValueOrDefault(),
                AuthorId = AuthorId,
                Name = Name,
                HtmlText = HtmlText,
                Approved = false,
                PromotedDateTimeUTC = DateTime.UtcNow
            };
        }

        public async Task<Post> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
