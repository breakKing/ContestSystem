﻿using CheckerSystemBaseStructures;
using ContestSystem.Models.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class TestBaseModel : BaseTest<ProblemBaseModel, VerdictModel>
    {
        public long Id { get; set; }
        public override short Number { get; set; }
        public long SolutionId { get; set; }
        public SolutionBaseModel Solution { get; set; }
        public override long UsedMemory { get; set; }
        public override int UsedTime { get; set; }
        public override short GotPoints { get; set; }
        public override short AvailablePoints { get; set; }


        [NotMapped]
        public override ProblemBaseModel Problem
        {
            get
            {
                return Solution.Problem;
            }
            set
            {

            }
        }

        [NotMapped]
        public override VerdictModel Result { get; set; }
        public VerdictType Verdict
        {
            get
            {
                return Result.Result;
            }
            set
            {
                Result.Result = value;
            }
        }
    }
}
