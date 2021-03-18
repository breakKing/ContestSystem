using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ContestInProgressOutputModel : IOutputModel<ContestBaseModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeLeft { get; set; }
        public string Type { get; set; }

        public void TransformForOutput(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            TimeSpan timeLeft = DateTime.Now - baseModel.StartDateTime;
            TimeLeft = timeLeft.ToString(@"hh\:mm\:ss");
            Type = baseModel.Type switch
            {
                ContestType.Competition => "Соревнование",
                ContestType.Training => "Тренировка",
                ContestType.Undefined => "Не определено",
                _ => "",
            };
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            TimeSpan timeLeft = DateTime.Now - baseModel.StartDateTime;
            TimeLeft = timeLeft.ToString(@"hh\:mm\:ss");
            Type = baseModel.Type switch
            {
                ContestType.Competition => "Соревнование",
                ContestType.Training => "Тренировка",
                ContestType.Undefined => "Не определено",
                _ => "",
            };
        }
    }
}
