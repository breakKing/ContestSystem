using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class PostBaseModel
    {
        public long Id { get; set; }
        public UserBaseModel Author { get; set; }
        public string Name { get; set; }
        public string HtmlText { get; set; }
        public DateTime PromotedDateTime { get; set; }
        public bool Approved { get; set; }
        public UserBaseModel ApprovingModerator { get; set; }
        public DateTime PublicationDateTime { get; set; }
    }
}
