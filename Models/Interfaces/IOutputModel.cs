using ContestSystemDbStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Interfaces
{
    interface IOutputModel<BaseModelType>
    {
        void TransformForOutput(BaseModelType baseModel, ContestSystemDbContext dbContext);
        Task TransformForOutputAsync(BaseModelType baseModel, ContestSystemDbContext dbContext);
    }
}
