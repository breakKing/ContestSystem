using System.Threading.Tasks;

namespace ContestSystem.Models.Interfaces
{
    public interface IOutputModel<BaseModelType>
    {
        public void TransformForOutput(BaseModelType baseModel);
        public Task TransformForOutputAsync(BaseModelType baseModel);
    }
}
