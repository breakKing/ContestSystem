using ContestSystem.Models.Base;
using ContestSystem.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class MonitorEntryOutputModel : IOutputModel<UserBaseModel>
    {
        public int Position { get; set; }

        public void TransformForOutput(UserBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }

        public Task TransformForOutputAsync(UserBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}
