using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class FutureContestOutputModel : IOutputModel<ContestBaseModel>
    {
        private readonly IStringLocalizer<ContestManagementOutputModel> _localizer;

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public DateTime EndDateTimeUTC { get; set; }
        public string Type { get; set; }
        public string CreatorUsername { get; set; }

        public FutureContestOutputModel(IStringLocalizer<ContestManagementOutputModel> localizer)
        {
            _localizer = localizer;
        }

        public void TransformForOutput(ContestBaseModel baseModel)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            StartDateTimeUTC = baseModel.StartDateTimeUTC;
            EndDateTimeUTC = baseModel.StartDateTimeUTC.AddMinutes(baseModel.DurationInMinutes);
            Type = baseModel.Type switch
            {
                ContestType.Competition => _localizer["CompetitionMode"],
                ContestType.Training => _localizer["TrainingMode"],
                ContestType.Undefined => "Undefined",
                _ => "",
            };
            CreatorUsername = baseModel.Creator.NormalizedUserName;
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
