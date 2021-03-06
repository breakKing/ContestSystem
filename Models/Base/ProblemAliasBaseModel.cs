﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ProblemAliasBaseModel
    {
        public ulong Id { get; set; }
        public ProblemBaseModel Problem { get; set; }
        public ContestBaseModel Contest { get; set; }
        public char Alias { get; set; }
    }
}
