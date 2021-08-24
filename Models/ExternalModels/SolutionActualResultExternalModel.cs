using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.ExternalModels
{
    public class SolutionActualResultExternalModel
    {
        public long SolutionId { get; set; }
        public long ContestId { get; set; }
        public short LastRunTestNumber { get; set; }
        public bool TestsAreDone { get; set; }
        public int UsedTimeInMillis { get; set; } // с последнего выполненного теста
        public long UsedMemoryInBytes { get; set; } // с последнего выполненного теста
        public VerdictType Verdict { get; set; }
        public short Points { get; set; }

        private static readonly List<VerdictType> _verdictsWhenTestsAreDone = new List<VerdictType>
        {
                VerdictType.Accepted,
                VerdictType.CompilationError,
                VerdictType.MemoryLimitExceeded,
                VerdictType.PartialSolution,
                VerdictType.PresentationError,
                VerdictType.RuntimeError,
                VerdictType.TestlibFail,
                VerdictType.TimeLimitExceeded,
                VerdictType.UnexpectedError
        };

        public static SolutionActualResultExternalModel GetFromModel(Solution solution)
        {
            if (solution == null)
            {
                return null;
            }

            short lastRunTestResultNumber = 0;
            TestResult lastRunTestResult = null;
            if (solution.TestResults != null && solution.TestResults.Count > 0)
            {
                lastRunTestResultNumber = solution.TestResults.Max(tr => tr.Number);
                lastRunTestResult = solution.TestResults.FirstOrDefault(tr => tr.Number == lastRunTestResultNumber);
            }
            return new SolutionActualResultExternalModel
            {
                SolutionId = solution.Id,
                ContestId = solution.ContestId,
                LastRunTestNumber = lastRunTestResultNumber,
                TestsAreDone = _verdictsWhenTestsAreDone.Contains(solution.Verdict),
                UsedTimeInMillis = lastRunTestResult?.UsedTimeInMillis ?? 0,
                UsedMemoryInBytes = lastRunTestResult?.UsedMemoryInBytes ?? 0,
                Points = solution.Points,
                Verdict = solution.Verdict
            };
        }
    }
}
