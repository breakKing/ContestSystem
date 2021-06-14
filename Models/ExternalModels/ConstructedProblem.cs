using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedProblem
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public List<ProblemLocalizerExternalModel> Localizers { get; set; } = new List<ProblemLocalizerExternalModel>();
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public string ModerationMessage { get; set; }
        public bool IsPublic { get; set; }
        public ConstructedChecker Checker { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public List<Test> Tests { get; set; }
        public List<Example> Examples { get; set; }

        public static ConstructedProblem GetFromModel(Problem problem)
        {
            return new ConstructedProblem
            {
                Id = problem.Id,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                ModerationMessage = problem.ModerationMessage,
                ApprovalStatus = problem.ApprovalStatus,
                ApprovingModerator = problem.ApprovingModerator?.ResponseStructure,
                Checker = ConstructedChecker.GetFromModel(problem.Checker),
                IsPublic = problem.IsPublic,
                Localizers = problem.ProblemLocalizers?.ConvertAll(pl => ProblemLocalizerExternalModel.GetFromModel(pl)),
                Tests = problem.Tests,
                Examples = problem.Examples
            };
        }
    }
}
