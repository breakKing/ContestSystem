using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ContestsModerators
    {
        public long Id { get; set; }
        public long ContestId { get; set; }
        public ContestBaseModel Contest { get; set; }
        public string ModeratorId { get; set; }
        public UserBaseModel Moderator { get; set; }
    }
}
