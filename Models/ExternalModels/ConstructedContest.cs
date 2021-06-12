using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ContestSystemDbStructure.Models.common;

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

        public static ConstructedContest GetFromModel(Contest contest, List<ContestProblem> problems)
        {
            return new ConstructedContest
            {
                Id = contest.Id,
                Localizers = contest.ContestLocalizers,
                Image = contest.Image,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                DurationInMinutes = contest.DurationInMinutes,
                Creator = contest.Creator?.ResponseStructure,
                ApprovalStatus = contest.ApprovalStatus,
                Rules = contest.RulesSet,
                ApprovingModerator = contest.ApprovingModerator?.ResponseStructure,
                ModerationMessage = contest.ModerationMessage,
                Problems = problems.ConvertAll(cp =>
                {
                    return new ProblemEntry
                    {
                        Letter = cp.Letter,
                        ProblemId = cp.ProblemId,
                        ContestId = cp.ContestId,
                        Problem = PublishedProblem.GetFromModel(cp.Problem,
                            cp.Problem.ProblemLocalizers.First(pl => pl.Culture == "ru")),
                    };
                })
            };
        }
    }
}