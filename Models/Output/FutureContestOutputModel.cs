using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class FutureContestOutputModel : IOutputModel<ContestBaseModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Type { get; set; }
        public string CreatorUsername { get; set; }
        
        public void TransformForOutput(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            StartDateTime = baseModel.StartDateTime.ToString("f");
            TimeSpan duration = new TimeSpan(0, baseModel.DurationInMinutes, 0);
            EndDateTime = baseModel.StartDateTime.Add(duration).ToString("f");
            Type = baseModel.Type switch
            {
                ContestType.Competition => "Соревнование",
                ContestType.Training => "Тренировка",
                ContestType.Undefined => "Не определено",
                _ => "",
            };
            CreatorUsername = baseModel.Creator.UserName;
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;
            Description = baseModel.Description;
            StartDateTime = baseModel.StartDateTime.ToString("f");
            TimeSpan duration = new TimeSpan(0, baseModel.DurationInMinutes, 0);
            EndDateTime = baseModel.StartDateTime.Add(duration).ToString("f");
            Type = baseModel.Type switch
            {
                ContestType.Competition => "Соревнование",
                ContestType.Training => "Тренировка",
                ContestType.Undefined => "Не определено",
                _ => "",
            };
            CreatorUsername = baseModel.Creator.UserName;
        }
    }
}
