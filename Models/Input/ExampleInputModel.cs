using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class ExampleInputModel : IInputModel<ExampleBaseModel, long?>
    {
        public long? Id { get; set; }
        public short Number { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public long ProblemId { get; set; }

        public ExampleBaseModel ReadFromInput()
        {
            return new ExampleBaseModel
            {
                Id = Id.GetValueOrDefault(),
                Number = Number,
                InputText = InputText,
                OutputText = OutputText,
                ProblemId = ProblemId
            };
        }

        public async Task<ExampleBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
