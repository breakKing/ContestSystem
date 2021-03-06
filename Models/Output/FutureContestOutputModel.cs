using ContestSystem.Models.Base;
using ContestSystem.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class FutureContestOutputModel : IOutputModel<ContestBaseModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Type { get; set; }
        public string CreatorUsername { get; set; }
        public void TransformForOutput(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }

        public Task TransformForOutputAsync(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}
