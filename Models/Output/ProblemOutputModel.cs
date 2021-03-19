using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ProblemOutputModel : IOutputModel<ProblemBaseModel>
    {
        public char Alias { get; set; }

        public void TransformForOutput(ProblemBaseModel baseModel)
        {
            throw new NotImplementedException();
        }

        public async Task TransformForOutputAsync(ProblemBaseModel baseModel)
        {
            throw new NotImplementedException();
        }
    }
}
