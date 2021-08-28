using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestWorkspaceModel
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
        public RulesSetBaseInfo Rules { get; set; }
        public bool AreVirtualContestsAvailable { get; set; }
        public List<ContestOrganizerExternalModel> Organizers { get; set; }

        public static ContestWorkspaceModel GetFromModel(Contest contest, string imageInBase64, Func<Problem, ProblemLocalizer> localizerPredicate)
        {
            if (contest == null)
            {
                return null;
            }

            return new ContestWorkspaceModel
            {
                Id = contest.Id,
                Localizers = contest.ContestLocalizers?.ConvertAll(ContestLocalizerExternalModel.GetFromModel),
                Image = imageInBase64,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                DurationInMinutes = contest.DurationInMinutes,
                Creator = contest.Creator?.ResponseStructure,
                ApprovalStatus = contest.ApprovalStatus,
                Rules = RulesSetBaseInfo.GetFromModel(contest.RulesSet),
                ApprovingModerator = contest.ApprovingModerator?.ResponseStructure,
                ModerationMessage = contest.ModerationMessage,
                AreVirtualContestsAvailable = contest.AreVirtualContestsAvailable,
                Problems = contest.ContestProblems?.ConvertAll(cp =>
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