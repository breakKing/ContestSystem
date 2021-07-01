using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedChecker
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public string Code { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public VerdictType CompilationVerdict { get; set; }
        public string Errors { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }

        public static ConstructedChecker GetFromModel(Checker checker)
        {
            return new ConstructedChecker
            {
                Id = checker.Id,
                Name = checker.Name,
                Description = checker.Description,
                CompilationVerdict = checker.CompilationVerdict,
                Errors = checker.Errors,
                IsPublic = checker.IsPublic,
                IsArchieved = checker.IsArchieved,
                Author = checker.Author?.ResponseStructure,
                ModerationMessage = checker.ModerationMessage,
                ApprovalStatus = checker.ApprovalStatus,
                ApprovingModerator = checker.ApprovingModerator?.ResponseStructure,
                Code = checker.Code
            };
        }
    }
}
