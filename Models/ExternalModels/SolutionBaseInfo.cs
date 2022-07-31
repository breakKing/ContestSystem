using ContestSystem.DbStructure.Models;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class SolutionBaseInfo
    {
        public long Id { get; set; }
        public ProblemBaseInfo Problem { get; set; }
        public object Participant { get; set; }
        public long? ContestId { get; set; }
        public long? CourseId { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public short Points { get; set; }
        public SolutionActualResultExternalModel ActualResult { get; set; }

        public static SolutionBaseInfo GetFromModel(Solution solution, ProblemLocalizer problemLocalizer)
        {
            if (solution == null)
            {
                return null;
            }

            return new SolutionBaseInfo
            {
                Id = solution.Id,
                Problem = ProblemBaseInfo.GetFromModel(solution.Problem, problemLocalizer),
                Participant = solution.Participant?.ResponseStructure,
                ContestId = solution.ContestId,
                CourseId = solution.CourseId,
                CompilerGUID = solution.CompilerGUID,
                CompilerName = solution.CompilerName,
                SubmitTimeUTC = solution.SubmitTimeUTC,
                Points = solution.Points,
                ActualResult = SolutionActualResultExternalModel.GetFromModel(solution)
            };
        }
    }
}
