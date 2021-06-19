using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedSolution
    {
        public long Id { get; set; }
        public PublishedProblem Problem { get; set; }
        public object Participant { get; set; }
        public long ContestId { get; set; }
        public string Code { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string ErrorsMessage { get; set; }
        public VerdictType Verdict { get; set; }
        public short Points { get; set; }
        public virtual List<TestResultExternalModel> TestResults { get; set; } = new List<TestResultExternalModel>();

        public static PublishedSolution GetFromModel(Solution solution, ProblemLocalizer problemLocalizer)
        {
            return new PublishedSolution
            {
                Id = solution.Id,
                Problem = PublishedProblem.GetFromModel(solution.Problem, problemLocalizer),
                Participant = solution.Participant?.ResponseStructure,
                ContestId = solution.ContestId,
                Code = solution.Code,
                CompilerGUID = solution.CompilerGUID,
                CompilerName = solution.CompilerName,
                SubmitTimeUTC = solution.SubmitTimeUTC,
                ErrorsMessage = solution.ErrorsMessage,
                Verdict = solution.Verdict,
                Points = solution.Points,
                TestResults = solution.TestResults.ConvertAll(tr => TestResultExternalModel.GetFromModel(tr))
            };
        }
    }
}
