using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestLocalizedModel
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
        public RulesSetBaseInfo Rules { get; set; }
        public List<ProblemEntry> Problems { get; set; }
        public bool AreVirtualContestsAvailable { get; set; }
        public List<ContestOrganizerExternalModel> Organizers { get; set; }

        public static ContestLocalizedModel GetFromModel(Contest contest, ContestLocalizer localizer, string imageInBase64, Func<Problem, ProblemLocalizer> localizerPredicate)
        {
            if (contest == null)
            {
                return null;
            }

            return new ContestLocalizedModel
            {
                Id = contest.Id,
                Creator = contest.Creator?.ResponseStructure,
                LocalizedDescription = localizer?.Description,
                LocalizedName = localizer?.Name,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                Image = imageInBase64,
                ParticipantsCount = contest.ContestParticipants?.Count ?? 0,
                Rules = RulesSetBaseInfo.GetFromModel(contest.RulesSet),
                AreVirtualContestsAvailable = contest.AreVirtualContestsAvailable,
                DurationInMinutes = contest.DurationInMinutes,
                Problems = contest.ContestProblems.ConvertAll(cp =>
                {
                    return new ProblemEntry
                    {
                        Letter = cp.Letter,
                        ProblemId = cp.ProblemId,
                        ContestId = cp.ContestId,
                        Problem = ProblemLocalizedModel.GetFromModel(cp.Problem,
                            localizerPredicate(cp.Problem))
                    };
                }),
                Organizers = contest.ContestOrganizers.ConvertAll(ContestOrganizerExternalModel.GetFromModel)
            };
        }
    }
}