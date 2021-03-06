﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Base
{
    public class ContestsParticipantsBaseModel
    {
        public long Id { get; set; }
        public ContestBaseModel Contest { get; set; }
        public UserBaseModel Participant { get; set; }
    }
}
