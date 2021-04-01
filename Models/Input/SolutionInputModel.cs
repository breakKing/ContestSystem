using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Enums;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class SolutionInputModel : IInputModel<SolutionBaseModel, long?>
    {
        public long? Id { get; set; }
        public long ProblemId { get; set; }
        public string ParticipantId { get; set; }
        public long ContestId { get; set; }
        public string Code { get; set; }
        public string Compiler { get; set; }

        public SolutionBaseModel ReadFromInput()
        {
            return new SolutionBaseModel
            {
                Id = Id.GetValueOrDefault(),
                ProblemId = ProblemId,
                ParticipantId = ParticipantId,
                ContestId = ContestId,
                Code = Code,
                Compiler = Compiler,
                SubmitTimeUTC = DateTime.UtcNow,
                Verdict = VerdictType.Undefined,
                ErrorsMessage = "",
                Points = 0
            };
        }

        public async Task<SolutionBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
