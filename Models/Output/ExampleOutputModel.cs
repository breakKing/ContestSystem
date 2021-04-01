using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ExampleOutputModel : IOutputModel<ExampleBaseModel>
    {
        public short Number { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }

        public void TransformForOutput(ExampleBaseModel baseModel)
        {
            Number = baseModel.Number;
            InputText = baseModel.InputText;
            OutputText = baseModel.OutputText;
        }

        public async Task TransformForOutputAsync(ExampleBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
