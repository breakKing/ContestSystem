using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class CheckerBaseInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public ApproveType ApprovalStatus { get; set; }

        public static CheckerBaseInfo GetFromModel(Checker checker)
        {
            if (checker == null)
            {
                return null;
            }

            return new CheckerBaseInfo
            {
                Id = checker.Id,
                Name = checker.Name,
                Description = checker.Description,
                IsPublic = checker.IsPublic,
                Author = checker.Author?.ResponseStructure,
                IsArchieved = checker.IsArchieved,
                ApprovalStatus = checker.ApprovalStatus
            };
        }
    }
}
