using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ContestManagementOutputModel : IOutputModel<ContestBaseModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public ContestType Type { get; set; }
        public ContestMode Mode { get; set; }
        public bool IsMonitorPublic { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public short DurationInMinutes { get; set; }
        public string CreatorId { get; set; }
        public virtual UserBaseModel Creator { get; set; }
        public bool Approved { get; set; }
        public string ApprovingGlobalContestModeratorId { get; set; }
        public virtual UserBaseModel ApprovingGlobalContestModerator { get; set; }

        public void TransformForOutput(ContestBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
