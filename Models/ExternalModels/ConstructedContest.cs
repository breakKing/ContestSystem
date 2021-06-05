using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedContest
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public List<ContestLocalizer> Localizers { get; set; }
        public string Image { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public DateTime EndDateTimeUTC { get; set; }
        public short DurationInMinutes { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
        public List<ProblemEntry> Problems { get; set; }
        public RulesSet Rules { get; set; }
    }
}
