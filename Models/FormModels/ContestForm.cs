using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class ContestForm
    {
        public long? Id { get; set; }
        [Required] public long CreatorUserId { get; set; } 
        public IFormFile Image { get; set; }
        [Required] public bool IsPublic { get; set; }
        [Required] public DateTime StartDateTimeUTC { get; set; }
        [Required] public short DurationInMinutes { get; set; }
        [Required] public bool AreVirtualContestsAvailable { get; set; }
        [Required] public List<ContestLocalizerForm> Localizers { get; set; }
        [Required] public List<ProblemEntryForm> Problems { get; set; }
        [Required] public long RulesSetId { get; set; }
    }
}
