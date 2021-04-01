using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class ContestInputModel : IInputModel<ContestBaseModel, long?>
    {
        public long? Id { get ; set ; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public ContestType Type { get; set; }
        public bool IsMonitorPublic { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public short DurationInMinutes { get; set; }
        public string CreatorId { get; set; }
        public ContestMode Mode { get; set; }

        public ContestBaseModel ReadFromInput()
        {
            return new ContestBaseModel
            {
                Id = Id.GetValueOrDefault(),
                Name = Name,
                Description = Description,
                IsPublic = IsPublic,
                Type = Type,
                Mode = Mode,
                IsMonitorPublic = IsMonitorPublic,
                StartDateTimeUTC = StartDateTimeUTC,
                DurationInMinutes = DurationInMinutes,
                CreatorId = CreatorId,
                Approved = false
            };
        }

        public async Task<ContestBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
