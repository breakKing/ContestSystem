using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ProblemEntryOutputModel : IOutputModel<ProblemBaseModel>
    {
        public void TransformForOutput(ProblemBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }

        public Task TransformForOutputAsync(ProblemBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
