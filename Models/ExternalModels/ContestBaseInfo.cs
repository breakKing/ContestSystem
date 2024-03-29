﻿using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using System;
using System.Collections.Generic;

namespace ContestSystem.Models.ExternalModels
{
    public class ContestBaseInfo
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
        public bool AreVirtualContestsAvailable { get; set; }
        public ApproveType ApprovalStatus { get; set; }
        public List<ContestOrganizerExternalModel> Organizers { get; set; }

        public static ContestBaseInfo GetFromModel(Contest contest, ContestLocalizer localizer, string imageInBase64)
        {
            if (contest == null)
            {
                return null;
            }

            return new ContestBaseInfo
            {
                Id = contest.Id,
                Creator = contest.Creator?.ResponseStructure,
                LocalizedDescription = localizer?.Description,
                LocalizedName = localizer?.Name,
                StartDateTimeUTC = contest.StartDateTimeUTC,
                EndDateTimeUTC = contest.EndDateTimeUTC,
                Image = imageInBase64,
                ParticipantsCount = contest.ContestParticipants?.Count ?? 0,
                AreVirtualContestsAvailable = contest.AreVirtualContestsAvailable,
                DurationInMinutes = contest.DurationInMinutes,
                ApprovalStatus = contest.ApprovalStatus,
                Organizers = contest.ContestOrganizers.ConvertAll(ContestOrganizerExternalModel.GetFromModel)
            };
        }
    }
}
