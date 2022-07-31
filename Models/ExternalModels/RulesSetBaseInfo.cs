using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    public class RulesSetBaseInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Author { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchieved { get; set; }
        public RulesCountMode CountMode { get; set; }
        public ApproveType ApprovalStatus { get; set; }

        public static RulesSetBaseInfo GetFromModel(RulesSet rules)
        {
            if (rules == null)
            {
                return null;
            }

            return new RulesSetBaseInfo
            {
                Id = rules.Id,
                Name = rules.Name,
                Description = rules.Description,
                Author = rules.Author?.ResponseStructure,
                IsPublic = rules.IsPublic,
                IsArchieved = rules.IsArchieved,
                CountMode = rules.CountMode,
                ApprovalStatus = rules.ApprovalStatus
            };
        }
    }
}
