﻿using ContestSystem.Areas.Messenger.Services;
using ContestSystem.Areas.Solutions.Services;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Contests.Services
{
    public class ContestsManagerService
    {
        private readonly SolutionsManagerService _solutionsManager;
        private readonly MessengerService _messenger;
        private readonly NotifierService _notifier;

        public ContestsManagerService(SolutionsManagerService solutionsManager, MessengerService messenger, NotifierService notifier)
        {
            _solutionsManager = solutionsManager;
            _messenger = messenger;
            _notifier = notifier;
        }

        public async Task<List<MonitorEntry>> GetContestMonitorAsync(MainDbContext dbContext, Contest contest)
        {
            var monitorEntries = new List<MonitorEntry>();

            if (contest == null)
            {
                return monitorEntries;
            }

            DateTime now = DateTime.UtcNow;

            var contestParticipants =
                await dbContext.ContestsParticipants.Where(cp => cp.ContestId == contest.Id).ToListAsync();

            var solutions = await dbContext.Solutions.Where(s => s.ContestId == contest.Id
                                                                  && (s.SubmitTimeUTC <
                                                                      contest.EndDateTimeUTC.AddMinutes(-contest
                                                                          .RulesSet
                                                                          .MonitorFreezeTimeBeforeFinishInMinutes)
                                                                      || contest.EndDateTimeUTC <= now))
                                                        .ToListAsync();

            var problems = contest.ContestProblems.OrderBy(cp => cp.Letter).ToList();

            foreach (var cp in contestParticipants)
            {
                var participantSolutions = solutions.Where(s => s.ParticipantId == cp.ParticipantId).ToList();
                
                var monitorEntry = GetMonitorEntryForParticipant(cp, problems, participantSolutions);

                monitorEntries.Add(monitorEntry);
            }

            if (contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
            {
                monitorEntries = monitorEntries.OrderByDescending(me => me.ProblemsSolvedCount)
                                                .ThenBy(me => me.Result)
                                                .ToList();
            }
            else
            {
                monitorEntries = monitorEntries.OrderByDescending(me => me.Result).ToList();
            }

            for (int i = 0; i < monitorEntries.Count; i++)
            {
                monitorEntries[i].Position = i + 1;
            }

            return monitorEntries;
        }

        public MonitorEntry GetMonitorEntryForParticipant(ContestParticipant participant, List<ContestProblem> problems, List<Solution> solutions)
        {
            if (participant == null)
            {
                return null;
            }

            var monitorEntry = new MonitorEntry
            {
                ContestId = participant.ContestId,
                UserId = participant.ParticipantId,
                Alias = participant.Alias,
                Position = 0,
                Result = _solutionsManager.GetResultForAllSolutions(solutions, participant.Contest),
                ProblemsSolvedCount = 0,
                ProblemTries = new List<ProblemTriesEntry>()
            };

            var contest = participant.Contest;

            foreach (var problem in problems)
            {
                var participantProblemSolutions = solutions
                    .Where(ps => ps.ProblemId == problem.ProblemId)
                    .OrderBy(ps => ps.SubmitTimeUTC)
                    .ToList();

                var problemTriesEntry = new ProblemTriesEntry
                {
                    ContestId = participant.ContestId,
                    UserId = participant.ParticipantId,
                    ProblemId = problem.ProblemId,
                    Letter = problem.Letter,
                    TriesCount = participantProblemSolutions.Count,
                };

                if (participantProblemSolutions == null || participantProblemSolutions.Count == 0)
                {
                    problemTriesEntry.LastTryMinutesAfterStart = 0;
                    problemTriesEntry.Solved = false;
                    problemTriesEntry.GotPoints = 0;
                }
                else
                {
                    problemTriesEntry.LastTryMinutesAfterStart =
                        (short)(participantProblemSolutions.Last().SubmitTimeUTC - contest.StartDateTimeUTC)
                        .TotalMinutes;
                    problemTriesEntry.Solved = participantProblemSolutions.Any(pps =>
                        pps.Verdict == VerdictType.Accepted || pps.Verdict == VerdictType.PartialSolution);

                    if (problemTriesEntry.Solved)
                    {
                        monitorEntry.ProblemsSolvedCount++;
                    }

                    problemTriesEntry.GotPoints = _solutionsManager.GetPointsForProblem(participantProblemSolutions, contest.RulesSet);
                }

                monitorEntry.ProblemTries.Add(problemTriesEntry);
            }

            return monitorEntry;
        }

        public async Task<List<Solution>> GetAllContestsSolutionsAsync(MainDbContext dbContext, Contest contest)
        {
            var solutions = new List<Solution>();

            if (contest != null)
            {
                solutions = await dbContext.Solutions.Where(s => s.ContestId == contest.Id)
                                                        .ToListAsync();
            }

            return solutions;
        }

        public async Task<FormCheckStatus> CheckSolutionManualVerdictFormAsync(MainDbContext dbContext, SolutionManualVerdictForm form)
        {
            var status = FormCheckStatus.Undefined;

            var solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == form.SolutionId);

            if (solution == null)
            {
                status = FormCheckStatus.NonExistentSolution;
            }
            else
            {
                bool canSetManualVerdict = solution.Verdict != VerdictType.TestInProgress
                                        && solution.Verdict != VerdictType.CompilationError
                                        && solution.Verdict != VerdictType.CompilationSucceed
                                        && solution.Verdict != VerdictType.Undefined;

                if (!canSetManualVerdict)
                {
                    status = FormCheckStatus.WrongMoment;
                }
                else
                {
                    status = FormCheckStatus.Correct;
                }
            }

            return status;
        }

        public async Task<EditionStatus> SetSolutionManualVerdictAsync(MainDbContext dbContext, SolutionManualVerdictForm form)
        {
            var status = EditionStatus.Undefined;

            var solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == form.SolutionId);

            if (solution != null)
            {
                bool changed = true;
                var rulesSet = solution.Contest.RulesSet;

                switch (form.Verdict)
                {
                    case VerdictType.WrongAnswer:
                        solution.Verdict = VerdictType.WrongAnswer;
                        solution.Points = 0;
                        break;
                    case VerdictType.PartialSolution:
                        if (rulesSet?.CountMode == RulesCountMode.CountPenalty)
                        {
                            solution.Verdict = VerdictType.Accepted;
                            solution.Points = 100;
                        }
                        else
                        {
                            solution.Verdict = VerdictType.PartialSolution;
                            solution.Points = form.Points;
                        }
                        break;
                    case VerdictType.Accepted:
                        solution.Verdict = VerdictType.Accepted;
                        solution.Points = 100;
                        break;
                    default:
                        changed = false;
                        break;
                }

                if (!changed)
                {
                    status = EditionStatus.Undefined;
                }
                else
                {
                    dbContext.Solutions.Update(solution);

                    bool saveSuccess = await dbContext.SecureSaveAsync();
                    if (!saveSuccess)
                    {
                        status = EditionStatus.DbSaveError;
                    }
                    else
                    {
                        status = EditionStatus.Success;
                        await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
                    }
                }
            }
            else
            {
                status = EditionStatus.NotExistentEntity;
            }

            return status;
        }

        public async Task<bool> IsUserContestOrganizerAsync(MainDbContext dbContext, long contestId, long userId)
        {
            return await dbContext.ContestsOrganizers.AnyAsync(co => co.ContestId == contestId
                                                                            && co.OrganizerId == userId);
        }

        public async Task<bool> IsUserContestParticipantAsync(MainDbContext dbContext, long contestId, long userId)
        {
            return await dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == contestId
                                                                        && cp.ParticipantId == userId
                                                                        && cp.ConfirmedByOrganizer
                                                                        && cp.ConfirmedByParticipant);
        }

        public async Task CreateContestInitialChatsAsync(MainDbContext dbContext, Contest contest)
        {
            if (contest != null && contest.CreatorId.HasValue)
            {
                var form = new ChatForm
                {
                    AdminId = contest.CreatorId.Value,
                    AnyoneCanJoin = false,
                    Image = null,
                    Name = "ContestAnnouncmentsChat",
                    InitialUsers = contest.ContestOrganizers.Select(co => co.OrganizerId).ToList()
                };

                var statusData = await _messenger.CreateChatAsync(dbContext, form, ChatType.ContestAnnouncements, contest.Id);

                if (statusData.Status != CreationStatus.Success)
                {
                    contest = await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contest.Id);
                    await CreateContestInitialChatsAsync(dbContext, contest);
                }
            }
        }

        public async Task AddParticipantToChatsAsync(MainDbContext dbContext, ContestParticipant participant)
        {
            if (participant != null)
            {
                var contest = participant.Contest;

                var announcmentsChatLink = (await dbContext.Chats.FirstOrDefaultAsync(c => c.Type == ChatType.ContestAnnouncements
                                                                                            && c.ContestId == participant.ContestId
                                                                                            && c.IsCreatedBySystem))
                                                                  .Link;

                await _messenger.AddUserToChatAsync(dbContext, participant.ParticipantId, announcmentsChatLink);

                var form = new ChatForm
                {
                    AdminId = contest.CreatorId.Value,
                    AnyoneCanJoin = false,
                    Image = null,
                    Name = $"ContestParticipant{participant.ParticipantId}Chat",
                    InitialUsers = contest.ContestOrganizers.Select(co => co.OrganizerId).Concat(new List<long> { participant.ParticipantId }).ToList()
                };

                await _messenger.CreateChatAsync(dbContext, form, ChatType.ContestParticipant, contest.Id, participant.ParticipantId);
            }
        }

        public async Task AddOrganizerToChatsAsync(MainDbContext dbContext, ContestOrganizer organizer)
        {
            if (organizer != null)
            {
                var contest = organizer.Contest;

                var announcmentsChatLink = (await dbContext.Chats.FirstOrDefaultAsync(c => c.Type == ChatType.ContestAnnouncements
                                                                                            && c.ContestId == organizer.ContestId
                                                                                            && c.IsCreatedBySystem))
                                                                  .Link;

                await _messenger.AddUserToChatAsync(dbContext, organizer.OrganizerId, announcmentsChatLink);

                var chats = await dbContext.Chats.Where(c => c.Type == ChatType.ContestParticipant
                                                                && c.ContestId == contest.Id)
                                                    .ToListAsync();

                foreach (var chat in chats)
                {
                    await _messenger.AddUserToChatAsync(dbContext, organizer.OrganizerId, chat.Link);
                }
            }
        }

        public async Task RemoveParticipantFromChatsAsync(MainDbContext dbContext, ContestParticipant participant)
        {
            if (participant != null)
            {
                var contest = participant.Contest;

                var chats = await dbContext.Chats.Where(c => c.ContestId == contest.Id
                                                                && c.IsCreatedBySystem
                                                                && c.ChatUsers.Any(cu => cu.UserId == participant.ParticipantId))
                                                    .ToListAsync();

                foreach (var chat in chats)
                {
                    if (chat.Type == ChatType.ContestParticipant)
                    {
                        await _messenger.DeleteChatAsync(dbContext, chat);
                    }
                    else
                    {
                        await _messenger.RemoveUserFromChatAsync(dbContext, chat, participant.ParticipantId);
                    }
                }
            }
        }

        public async Task RemoveOrganizerFromChatsAsync(MainDbContext dbContext, ContestOrganizer organizer)
        {
            if (organizer != null)
            {
                var contest = organizer.Contest;

                var chats = await dbContext.Chats.Where(c => c.ContestId == contest.Id
                                                                && c.IsCreatedBySystem
                                                                && c.ChatUsers.Any(cu => cu.UserId == organizer.OrganizerId))
                                                    .ToListAsync();

                foreach (var chat in chats)
                {
                    await _messenger.RemoveUserFromChatAsync(dbContext, chat, organizer.OrganizerId);
                }
            }
        }

        public async Task RemoveAllContestChatsAsync(MainDbContext dbContext, long contestId)
        {
            var chats = await dbContext.Chats.Where(c => c.ContestId == contestId
                                                            && c.IsCreatedBySystem)
                                                .ToListAsync();

            if (chats != null && chats.Count > 0)
            {
                dbContext.Chats.RemoveRange(chats);
                
                if (!await dbContext.SecureSaveAsync())
                {
                    await RemoveAllContestChatsAsync(dbContext, contestId);
                }
            }
        }

        public async Task<bool> InitialChatsExistAsync(MainDbContext dbContext, long contestId)
        {
            return await dbContext.Chats.AnyAsync(c => c.ContestId == contestId
                                                    && c.IsCreatedBySystem);
        }

        public async Task<List<ChatExternalModel>> GetUserContestChatsAsync(MainDbContext dbContext, long contestId, long userId)
        {
            var externalChats = new List<ChatExternalModel>();

            var contest = await dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return externalChats;
            }

            var chats = await dbContext.Chats.Where(c => c.IsCreatedBySystem
                                                            && c.ContestId == contest.Id
                                                            && c.ChatUsers.Any(cu => cu.UserId == userId))
                                                .ToListAsync();

            return chats.ConvertAll(c => _messenger.GetChatHistoryAsync(dbContext, c.Link, null, null).GetAwaiter().GetResult());
        }

        public async Task<bool> ContestHasSolutionAsync(MainDbContext dbContext, Contest contest, long solutionId)
        {
            if (contest == null)
            {
                return false;
            }

            return await dbContext.Solutions.AnyAsync(s => s.Id == solutionId && s.ContestId == contest.Id);
        }

        public async Task<DeletionStatus> DeleteSolutionAsync(MainDbContext dbContext, Contest contest, long solutionId)
        {
            var status = DeletionStatus.Undefined;

            var solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solutionId && s.ContestId == contest.Id);

            if (solution == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                if (solution.Verdict != VerdictType.UnexpectedError 
                    && solution.Verdict != VerdictType.TestlibFail
                    && solution.Verdict != VerdictType.CheckerServersUnavailable)
                {
                    status = DeletionStatus.Blocked;
                }
                else
                {
                    dbContext.Solutions.Remove(solution);
                    bool saveSuccess = await dbContext.SecureSaveAsync();

                    if (!saveSuccess)
                    {
                        status = DeletionStatus.DbSaveError;
                    }
                    else
                    {
                        status = DeletionStatus.Success;
                    }
                }
            }

            return status;
        }
    }
}
