using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class NotificationOutputModel : IOutputModel<NotificationBaseModel>
    {
        private readonly IStringLocalizer<NotificationOutputModel> _localizer;

        public string Type { get; set; }
        public DateTime GenerationDateTimeUTC { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }

        public NotificationOutputModel(IStringLocalizer<NotificationOutputModel> localizer)
        {
            _localizer = localizer;
        }

        public void TransformForOutput(NotificationBaseModel baseModel)
        {
            Type = baseModel.Type switch
            {
                NotificationType.ParticipationInContest => _localizer["ParticipationInContest"],
                NotificationType.ContestManagement => _localizer["ContestManagement"],
                NotificationType.PostManagement => _localizer["PostManagement"],
                NotificationType.SystemManagent => _localizer["SystemManagent"],
                NotificationType.Undefined => "Undefined",
                _ => "",
            };
            GenerationDateTimeUTC = baseModel.GenerationDateTimeUTC;
            Text = baseModel.Text;
            IsRead = baseModel.IsRead;
        }

        public async Task TransformForOutputAsync(NotificationBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
