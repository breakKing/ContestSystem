﻿using CheckerSystemBaseStructures;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class SolutionBaseModel
    {
        public ulong Id { get; set; }
        public ProblemBaseModel Problem { get; set; }
        public UserBaseModel Participant { get; set; }
        public ContestBaseModel Contest { get; set; }
        public string Code { get; set; }
        public string Compiler { get; set; }
        public DateTime SubmitTime { get; set; }
        public string ErrorsMessage { get; set; }
    }
}
