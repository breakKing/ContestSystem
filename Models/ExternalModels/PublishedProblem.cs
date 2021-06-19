﻿using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;

namespace ContestSystem.Models.ExternalModels
{
    /**
     * Пришлось вынести чтобы иметь доступ к пространству имён в основной модели
     */
    public class PublishedProblem
    {
        public long Id { get; set; }
        public object Creator { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public string LocalizedInputBlock { get; set; }
        public string LocalizedOutputBlock { get; set; }
        public long MemoryLimitInBytes { get; set; }
        public int TimeLimitInMilliseconds { get; set; }
        public string ModerationMessage { get; set; }
        public ApproveType ApprovalStatus { get; set; }

        public static PublishedProblem GetFromModel(Problem problem, ProblemLocalizer localizer)
        {
            return new PublishedProblem
            {
                Id = problem.Id,
                LocalizedName = localizer?.Name,
                LocalizedDescription = localizer?.Description,
                LocalizedInputBlock = localizer?.InputBlock,
                LocalizedOutputBlock = localizer?.OutputBlock,
                MemoryLimitInBytes = problem.MemoryLimitInBytes,
                TimeLimitInMilliseconds = problem.TimeLimitInMilliseconds,
                Creator = problem.Creator?.ResponseStructure,
                ModerationMessage = problem.ModerationMessage,
                ApprovalStatus = problem.ApprovalStatus
            };
        }
    }
}