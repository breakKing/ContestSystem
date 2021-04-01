using ContestSystem.Models.Constants;
using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class TestInputModel : IInputModel<TestBaseModel, long?>
    {
        public long? Id { get; set; }
        public short Number { get; set; }
        public long ProblemId { get; set; }
        public string Input { get; set; }
        public string Answer { get; set; }
        public short? AvailablePoints { get; set; }

        public TestBaseModel ReadFromInput()
        {
            return new TestBaseModel
            {
                Id = Id.GetValueOrDefault(),
                Number = Number,
                ProblemId = ProblemId,
                Input = Input,
                Answer = Answer,
                AvailablePoints = AvailablePoints.GetValueOrDefault(SystemConstants.maxPointsForProblem)
            };
        }

        public async Task<TestBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
