using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedSolution
    {
        public long Id { get; set; }
        public PublishedProblem Problem { get; set; }
        public object Participant { get; set; }
        public PublishedContest Contest { get; set; }
        public string Code { get; set; }
        public string CompilerName { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string ErrorsMessage { get; set; }
        public VerdictType Verdict { get; set; }
        public short Points { get; set; }

        public static PublishedSolution GetFromModel(Solution solution, ContestLocalizer contestLocalizer, ProblemLocalizer problemLocalizer, int participantsCount)
        {
            return new PublishedSolution
            {
                Id = solution.Id,
                Participant = solution.Participant?.ResponseStructure,
                Contest = PublishedContest.GetFromModel(solution.Contest, contestLocalizer, participantsCount),
                Problem = PublishedProblem.GetFromModel(solution.Problem, problemLocalizer),
                Code = solution.Code,
                CompilerName = solution.CompilerName,
                SubmitTimeUTC = solution.SubmitTimeUTC,
                ErrorsMessage = solution.ErrorsMessage,
                Verdict = solution.Verdict,
                Points = solution.Points
            };
        }
    }
}
