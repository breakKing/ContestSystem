using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class PostInputModel : IInputModel<PostBaseModel, long?>
    {
        public long? Id { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
        public string HtmlText { get; set; }

        public PostBaseModel ReadFromInput()
        {
            return new PostBaseModel
            {
                Id = Id.GetValueOrDefault(),
                AuthorId = AuthorId,
                Name = Name,
                HtmlText = HtmlText,
                Approved = false,
                PromotedDateTime = DateTime.Now
            };
        }

        public async Task<PostBaseModel> ReadFromInputAsync()
        {
            return new PostBaseModel
            {
                Id = Id.GetValueOrDefault(),
                AuthorId = AuthorId,
                Name = Name,
                HtmlText = HtmlText,
                Approved = false,
                PromotedDateTime = DateTime.Now
            };
        }
    }
}
