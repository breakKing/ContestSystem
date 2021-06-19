using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ConstructedRulesSet Rules { get; set; }
        public List<ProblemEntry> Problems { get; set; }

        public static PublishedContest GetFromModel(Contest contest, ContestLocalizer localizer, int participantsCount)
        {
            return new PublishedContest
            {
                Id = contest.Id,
                Creator = contest.Creator?.ResponseStructure,
                LocalizedDescription = localizer.Description,
                LocalizedName = localizer.Name,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                Image = contest.Image,
                ParticipantsCount = participantsCount,
                ApprovalStatus = contest.ApprovalStatus,
                Rules = ConstructedRulesSet.GetFromModel(contest.RulesSet),
                Problems = contest.ContestProblems.ConvertAll(cp =>
                {
                    return new ProblemEntry
                    {
                        Letter = cp.Letter,
                        ProblemId = cp.ProblemId,
                        ContestId = cp.ContestId,
                        Problem = PublishedProblem.GetFromModel(cp.Problem,
                            cp.Problem.ProblemLocalizers.First(pl => pl.Culture == localizer.Culture)),
                    };
                })
            };
        }
    }
}