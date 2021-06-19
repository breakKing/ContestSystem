using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedSolution
    {
        public long Id { get; set; }
        public ConstructedProblem Problem { get; set; }
        public object Participant { get; set; }
        public ConstructedContest Contest { get; set; }
        public string Code { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string ErrorsMessage { get; set; }
        public VerdictType Verdict { get; set; }
        public short Points { get; set; }
        public List<TestResultExternalModel> TestResults { get; set; } = new List<TestResultExternalModel>();

        public static ConstructedSolution GetFromModel(Solution solution, List<ContestProblem> problemsInContest)
        {
            return new ConstructedSolution
            {
                Id = solution.Id,
                Participant = solution.Participant?.ResponseStructure,
                Contest = ConstructedContest.GetFromModel(solution.Contest, problemsInContest),
                Problem = ConstructedProblem.GetFromModel(solution.Problem),
                Code = solution.Code,
                CompilerGUID = solution.CompilerGUID,
                CompilerName = solution.CompilerName,
                SubmitTimeUTC = solution.SubmitTimeUTC,
                ErrorsMessage = solution.ErrorsMessage,
                Verdict = solution.Verdict,
                Points = solution.Points,
                TestResults = solution.TestResults?.ConvertAll(tr => TestResultExternalModel.GetFromModel(tr))
            };
        }
    }
}
