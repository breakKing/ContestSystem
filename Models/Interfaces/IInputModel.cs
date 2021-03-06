﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Interfaces
{
    interface IInputModel<BaseModelType>
    {
        BaseModelType ReadFromInput();
    }
}