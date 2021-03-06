using ContestSystem.Models.Base;
using ContestSystem.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ProblemOutputModel : IOutputModel<ProblemBaseModel>
    {
        public char Alias { get; set; }

        public void TransformForOutput(ProblemBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }

        public Task TransformForOutputAsync(ProblemBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}
