using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class CheckerWorkspaceModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public string Code { get; set; }
        public bool IsPublic { get; set; }
        public VerdictType CompilationVerdict { get; set; }
        public string Errors { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }

        public static CheckerWorkspaceModel GetFromModel(Checker checker)
        {
            if (checker == null)
            {
                return null;
            }

            return new CheckerWorkspaceModel
            {
                Id = checker.Id,
                Name = checker.Name,
                Description = checker.Description,
                CompilationVerdict = checker.CompilationVerdict,
                Errors = checker.Errors,
                IsPublic = checker.IsPublic,
                Author = checker.Author?.ResponseStructure,
                ModerationMessage = checker.ModerationMessage,
                ApprovalStatus = checker.ApprovalStatus,
                ApprovingModerator = checker.ApprovingModerator?.ResponseStructure,
                Code = checker.Code
            };
        }
    }
}
