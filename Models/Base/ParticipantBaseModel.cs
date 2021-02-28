using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ParticipantBaseModel
    {
        public long Id { get; set; }
        public IdentityUser User { get; set; }
        public List<SolutionBaseModel> Solutions { get; set; }
    }
}
