using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class PostOutputModel : IOutputModel<PostBaseModel>
    {
        public void TransformForOutput(PostBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }

        public async Task TransformForOutputAsync(PostBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
