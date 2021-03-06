using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ContestsProblemsBaseModel
    {
        public long Id { get; set; }
        public long ContestId { get; set; }
        public ContestBaseModel Contest { get; set; }
        public long ProblemId { get; set; }
        public ProblemBaseModel Problem { get; set; }
        public char Alias { get; set; }
    }
}
