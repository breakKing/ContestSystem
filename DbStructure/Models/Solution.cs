using ContestSystem.DbStructure.Enums;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using ContestSystem.DbStructure.Models.Auth;

namespace ContestSystem.DbStructure.Models
{
    public class Solution : BaseEntity
    {
        public long ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public long? ParticipantId { get; set; }
        [JsonIgnore] public virtual User Participant { get; set; }
        public long? ContestId { get; set; }
        [JsonIgnore] public virtual Contest Contest { get; set; }
        public long? CourseId { get; set; }
        [JsonIgnore] public virtual Course Course { get; set; }
        public string Code { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
        public long? CheckerServerId { get; set; }
        [JsonIgnore] public virtual CheckerServer CheckerServer { get; set; }
        public DateTime SubmitTimeUTC { get; set; }
        public string ErrorsMessage { get; set; }
        public VerdictType Verdict { get; set; }
        public short Points { get; set; }
        [JsonIgnore] public virtual List<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}