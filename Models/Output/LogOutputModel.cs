using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class LogOutputModel : IOutputModel<LogBaseModel>
    {
        public string Class { get; set; }
        public string Severity { get; set; }
        public string Text { get; set; }
        public string EventDateTime { get; set; }

        public void TransformForOutput(LogBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Class = baseModel.Class;
            Severity = baseModel.Severity;
            Text = baseModel.Text;
            EventDateTime = baseModel.EventDateTime.ToString("G");
        }

        public async Task TransformForOutputAsync(LogBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Class = baseModel.Class;
            Severity = baseModel.Severity;
            Text = baseModel.Text;
            EventDateTime = baseModel.EventDateTime.ToString("G");
        }
    }
}
