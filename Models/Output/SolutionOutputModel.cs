﻿using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class SolutionOutputModel : IOutputModel<SolutionBaseModel>
    {
        public void TransformForOutput(SolutionBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }

        public Task TransformForOutputAsync(SolutionBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }
    }
}