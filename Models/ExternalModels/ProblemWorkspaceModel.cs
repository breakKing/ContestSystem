using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ProblemWorkspaceModel
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public List<ProblemLocalizerExternalModel> Localizers { get; set; } = new List<ProblemLocalizerExternalModel>();
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public string ModerationMessage { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public CheckerBaseInfo Checker { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public List<Test> Tests { get; set; }
        public List<Example> Examples { get; set; }

        public static ProblemWorkspaceModel GetFromModel(Problem problem)
        {
            if (problem == null)
            {
                return null;
            }

            return new ProblemWorkspaceModel
            {
                Id = problem.Id,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                ModerationMessage = problem.ModerationMessage,
                ApprovalStatus = problem.ApprovalStatus,
                ApprovingModerator = problem.ApprovingModerator?.ResponseStructure,
                Checker = CheckerBaseInfo.GetFromModel(problem.Checker),
                IsPublic = problem.IsPublic,
                IsArchieved = problem.IsArchieved,
                Localizers = problem.ProblemLocalizers?.ConvertAll(ProblemLocalizerExternalModel.GetFromModel),
                Tests = problem.Tests,
                Examples = problem.Examples
            };
        }
    }
}
