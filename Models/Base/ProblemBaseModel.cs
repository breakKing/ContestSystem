using CheckerSystemBaseStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ProblemBaseModel : BaseProblem
    {
        public override ulong Id { get; set; }
        public override uint MemoryLimit { get; set; }
        public override uint TimeLimit { get; set; }
        public override ProblemType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputBlock { get; set; }
        public string OutputBlock { get; set; }
        public bool IsPublic { get; set; }
    }
}
