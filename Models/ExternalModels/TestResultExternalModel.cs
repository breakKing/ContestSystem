using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;

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
        public string CheckerOutput { get; set; }

        public static TestResultExternalModel GetFromModel(TestResult testResult, bool includeCheckerData = false)
        {
            if (testResult == null)
            {
                return null;
            }

            var result =  new TestResultExternalModel
            {
                Number = testResult.Number,
                Input = testResult.Input,
                Output = testResult.Output,
                SolutionId = testResult.SolutionId,
                UsedMemoryInBytes = testResult.UsedMemoryInBytes,
                UsedTimeInMillis = testResult.UsedTimeInMillis,
                GotPoints = testResult.GotPoints,
                Verdict = testResult.Verdict,
                CheckerOutput = null
            };

            if (includeCheckerData)
            {
                result.CheckerOutput = testResult.CheckerOutput;
            }

            return result;
        }
    }
}
