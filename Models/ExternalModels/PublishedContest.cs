using ContestSystemDbStructure.Enums;
using System;

namespace ContestSystem.Models.ExternalModels
{
    public class PublishedContest
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public string Image { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public DateTime EndDateTimeUTC { get; set; }
        public int ParticipantsCount { get; set; }
        public short DurationInMinutes { get; set; }
        public string ModerationMessage { get; set; }
        public ApproveType ApprovalStatus { get; set; }
    }
}
