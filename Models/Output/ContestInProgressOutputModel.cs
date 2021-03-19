using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ContestInProgressOutputModel : IOutputModel<ContestBaseModel>
    {
        private readonly IStringLocalizer<ContestInProgressOutputModel> _localizer;

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EndTimeUTC { get; set; }
        public string Type { get; set; }

        public ContestInProgressOutputModel(IStringLocalizer<ContestInProgressOutputModel> localizer)
        {
            _localizer = localizer;
        }

        public void TransformForOutput(ContestBaseModel baseModel)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            EndTimeUTC = baseModel.StartDateTimeUTC.AddMinutes(baseModel.DurationInMinutes);
            Type = baseModel.Type switch
            {
                ContestType.Competition => _localizer["CompetitionMode"],
                ContestType.Training => _localizer["TrainingMode"],
                ContestType.Undefined => "Undefined",
                _ => "",
            };
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
