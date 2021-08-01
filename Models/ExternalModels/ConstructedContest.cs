using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.ExternalModels
{
    public class ConstructedContest
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public List<ContestLocalizerExternalModel> Localizers { get; set; } = new List<ContestLocalizerExternalModel>();
        public string Image { get; set; }
        public DateTime StartDateTimeUTC { get; set; }
        public DateTime EndDateTimeUTC { get; set; }
        public short DurationInMinutes { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public object ApprovingModerator { get; set; }
        public string ModerationMessage { get; set; }
        public List<ProblemEntry> Problems { get; set; }
        public ConstructedRulesSet Rules { get; set; }
        public long? RulesSetId { get; set; }
        public bool AreVirtualContestsAvailable { get; set; }

        public static ConstructedContest GetFromModel(Contest contest, List<ContestProblem> problems, string imageInBase64)
        {
            return new ConstructedContest
            {
                Id = contest.Id,
                Localizers = contest.ContestLocalizers?.ConvertAll(ContestLocalizerExternalModel.GetFromModel),
                Image = imageInBase64,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                DurationInMinutes = contest.DurationInMinutes,
                Creator = contest.Creator?.ResponseStructure,
                ApprovalStatus = contest.ApprovalStatus,
                RulesSetId = contest.RulesSet.Id,
                Rules = ConstructedRulesSet.GetFromModel(contest.RulesSet),
                ApprovingModerator = contest.ApprovingModerator?.ResponseStructure,
                ModerationMessage = contest.ModerationMessage,
                AreVirtualContestsAvailable = contest.AreVirtualContestsAvailable,
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