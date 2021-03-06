using CheckerSystemBaseStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ProblemBaseModel : BaseProblem
    {
        public override long Id { get; set; }
        public override long MemoryLimit { get; set; }
        public override int TimeLimit { get; set; }
        public override ProblemType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputBlock { get; set; }
        public string OutputBlock { get; set; }
        public bool IsPublic { get; set; }
    }
}
