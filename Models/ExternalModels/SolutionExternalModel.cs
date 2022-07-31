using ContestSystem.DbStructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.ExternalModels
{
    public class SolutionExternalModel
    {
        public long Id { get; set; }
        public ProblemBaseInfo Problem { get; set; }
        public object Participant { get; set; }
        public ContestBaseInfo Contest { get; set; }
        public string Code { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string ErrorsMessage { get; set; }
        public List<TestResultExternalModel> TestResults { get; set; } = new List<TestResultExternalModel>();
        public SolutionActualResultExternalModel ActualResult { get; set; }

        public static SolutionExternalModel GetFromModel(Solution solution, string contestImageInBase64,
            ContestLocalizer contestLocalizer, ProblemLocalizer problemLocalizer,
            bool testsResultsRestricted = true, bool includeCheckerData = false)
        {
            if (solution == null)
            {
                return null;
            }

            var neededTestResults = new List<TestResultExternalModel>();
            if (solution.TestResults != null && solution.TestResults.Count > 0)
            {
                if (!testsResultsRestricted || (solution.Contest?.RulesSet?.ShowFullTestsResults ?? false))
                {
                    neededTestResults = solution.TestResults.ConvertAll(tr => TestResultExternalModel.GetFromModel(tr, includeCheckerData));
                }
                else
                {
                    var maxNumber = solution.TestResults.Max(tr => tr.Number);
                    var testResult = TestResultExternalModel.GetFromModel(solution.TestResults.FirstOrDefault(t => t.Number == maxNumber), includeCheckerData);
                    neededTestResults.Add(testResult);
                }
            }

            return new SolutionExternalModel
            {
                Id = solution.Id,
                Participant = solution.Participant?.ResponseStructure,
                Contest = ContestBaseInfo.GetFromModel(solution.Contest, contestLocalizer, contestImageInBase64),
                Problem = ProblemBaseInfo.GetFromModel(solution.Problem, problemLocalizer),
                Code = solution.Code,
                CompilerGUID = solution.CompilerGUID,
                CompilerName = solution.CompilerName,
                SubmitTimeUTC = solution.SubmitTimeUTC,
                ErrorsMessage = solution.ErrorsMessage,
                TestResults = neededTestResults,
                ActualResult = SolutionActualResultExternalModel.GetFromModel(solution)
            };
        }
    }
}
