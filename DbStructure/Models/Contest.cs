using ContestSystem.DbStructure.Models.Auth;
using ContestSystem.DbStructure.Models.Messenger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ContestSystem.DbStructure.Models
{
    public class Contest : BaseEvent
    {
        public long? RulesSetId { get; set; }
        public virtual RulesSet RulesSet { get; set; }
        public DateTime StartDateTimeUTC { get; set; }

        [NotMapped]
        [JsonInclude]
        public DateTime EndDateTimeUTC
        {
            get { return StartDateTimeUTC.AddMinutes(DurationInMinutes); }
        }

        public short DurationInMinutes { get; set; }
        public string ImagePath { get; set; }
        public bool AreVirtualContestsAvailable { get; set; }
        public virtual List<ContestLocalizer> ContestLocalizers { get; set; }
        public virtual List<Chat> Chats { get; set; }
        public virtual List<ContestFile> ContestFiles { get; set; }
        public virtual List<ContestHistory> ContestHistories { get; set; }
        public virtual List<Solution> Solutions { get; set; }

        public virtual List<ContestProblem> ContestProblems { get; set; }
        public virtual List<Problem> Problems { get; set; }

        public virtual List<ContestParticipant> ContestParticipants { get; set; }
        public virtual List<User> Participants { get; set; }

        public virtual List<ContestOrganizer> ContestOrganizers { get; set; }
        public virtual List<User> Organizers { get; set; }
    }
}