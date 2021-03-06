using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ExampleBaseModel
    {
        public ulong Id { get; set; }
        public uint Number { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public ProblemBaseModel Problem { get; set; }
    }
}
