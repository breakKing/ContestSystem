﻿using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class MessageOutputModel : IOutputModel<MessageBaseModel>
    {
        public void TransformForOutput(MessageBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }

        public Task TransformForOutputAsync(MessageBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }
    }
}