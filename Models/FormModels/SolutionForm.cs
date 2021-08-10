using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class SolutionForm
    {
        [Required] public string Code { get; set; }
        [Required] public string CompilerGUID { get; set; }
        [Required] public string CompilerName { get; set; }
        [Required] public long ContestId { get; set; }
        [Required] public long UserId { get; set; }
        [Required] public long ProblemId { get; set; }
    }
}
