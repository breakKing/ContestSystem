using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Input
{
    public class ProblemInputModel : IInputModel<ProblemBaseModel, long?>
    {
        public long? Id { get; set; }
        public long MemoryLimitInMegaBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public ProblemType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputBlock { get; set; }
        public string OutputBlock { get; set; }
        public bool IsPublic { get; set; }
        public string CheckerCode { get; set; }

        public ProblemBaseModel ReadFromInput()
        {
            return new ProblemBaseModel
            {
                Id = Id.GetValueOrDefault(),
                MemoryLimitInBytes = MemoryLimitInMegaBytes * 1024 * 1024,
                TimeLimitInMilliseconds = TimeLimitInMilliseconds,
                Type = Type,
                Name = Name,
                Description = Description,
                InputBlock = InputBlock,
                OutputBlock = OutputBlock,
                IsPublic = IsPublic,
                CheckerCode = CheckerCode,
                Approved = false
            };
        }

        public async Task<ProblemBaseModel> ReadFromInputAsync()
        {
            return ReadFromInput();
        }
    }
}
