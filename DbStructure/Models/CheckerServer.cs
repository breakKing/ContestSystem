using System;
using System.Collections.Generic;

namespace ContestSystem.DbStructure.Models
{
    public class CheckerServer: BaseEntity
    {
        public string Address { get; set; }
        public DateTime LastTimeUsedForSolutionUTC { get; set; }
        public DateTime LastTimeCompilersUpdatedUTC { get; set; }
        public virtual List<Solution> Solutions { get; set; }
        public virtual List<CheckerServerCompiler> CheckerServerCompilers { get; set; }
    }
}
