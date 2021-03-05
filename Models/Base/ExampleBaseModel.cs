﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ExampleBaseModel
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public ProblemBaseModel Problem { get; set; }
    }
}