using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class TestResultExternalModel
    {
        public short Number { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public long SolutionId { get; set; }
        public long UsedMemoryInBytes { get; set; }
        public int UsedTimeInMillis { get; set; }
        public short GotPoints { get; set; }
        public VerdictType Verdict { get; set; }

        public static TestResultExternalModel GetFromModel(TestResult testResult)
        {
            return new TestResultExternalModel
            {
                Number = testResult.Number,
                Input = testResult.Input,
                Output = testResult.Output,
                SolutionId = testResult.SolutionId,
                UsedMemoryInBytes = testResult.UsedMemoryInBytes,
                UsedTimeInMillis = testResult.UsedTimeInMillis,
                GotPoints = testResult.GotPoints,
                Verdict = testResult.Verdict
            };
        }
    }
}
