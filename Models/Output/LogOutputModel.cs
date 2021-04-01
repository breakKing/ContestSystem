using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class LogOutputModel : IOutputModel<LogBaseModel>
    {
        public string Class { get; set; }
        public string Severity { get; set; }
        public string Text { get; set; }
        public DateTime EventDateTimeUTC { get; set; }

        public void TransformForOutput(LogBaseModel baseModel)
        {
            Class = baseModel.Class;
            Severity = baseModel.Severity;
            Text = baseModel.Text;
            EventDateTimeUTC = baseModel.EventDateTimeUTC;
        }

        public async Task TransformForOutputAsync(LogBaseModel baseModel)
        {
            TransformForOutput(baseModel);
        }
    }
}
