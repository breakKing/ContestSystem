using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ProblemEntryOutputModel : IOutputModel<ProblemBaseModel>
    {
        public void TransformForOutput(ProblemBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }

        public async Task TransformForOutputAsync(ProblemBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
