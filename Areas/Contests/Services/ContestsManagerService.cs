using ContestSystem.Areas.Messenger.Services;
using ContestSystem.Areas.Solutions.Services;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
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

        public ContestsManagerService(SolutionsManagerService solutionsManager, MessengerService messenger)
        {
            _solutionsManager = solutionsManager;
            _messenger = messenger;
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

        public async Task<bool> IsUserContestLocalModeratorAsync(MainDbContext dbContext, long contestId, long userId)
        {
            return await dbContext.ContestsLocalModerators.AnyAsync(clm => clm.ContestId == contestId
                                                                            && clm.LocalModeratorId == userId);
        }

        public async Task<bool> IsUserContestParticipantAsync(MainDbContext dbContext, long contestId, long userId)
        {
            return await dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == contestId
                                                                        && cp.ParticipantId == userId
                                                                        && cp.ConfirmedByLocalModerator
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
                    InitialUsers = contest.ContestLocalModerators.Select(clm => clm.LocalModeratorId).ToList()
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
                    InitialUsers = contest.ContestLocalModerators.Select(clm => clm.LocalModeratorId).Concat(new List<long> { participant.ParticipantId }).ToList()
                };

                await _messenger.CreateChatAsync(dbContext, form, ChatType.ContestParticipant, contest.Id, participant.ParticipantId);
            }
        }

        public async Task AddLocalModeratorToChatsAsync(MainDbContext dbContext, ContestLocalModerator localModerator)
        {
            if (localModerator != null)
            {
                var contest = localModerator.Contest;

                var announcmentsChatLink = (await dbContext.Chats.FirstOrDefaultAsync(c => c.Type == ChatType.ContestAnnouncements
                                                                                            && c.ContestId == localModerator.ContestId
                                                                                            && c.IsCreatedBySystem))
                                                                  .Link;

                await _messenger.AddUserToChatAsync(dbContext, localModerator.LocalModeratorId, announcmentsChatLink);

                var chats = await dbContext.Chats.Where(c => c.Type == ChatType.ContestParticipant
                                                                && c.ContestId == contest.Id)
                                                    .ToListAsync();

                foreach (var chat in chats)
                {
                    await _messenger.AddUserToChatAsync(dbContext, localModerator.LocalModeratorId, chat.Link);
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
                    await _messenger.RemoveUserFromChatAsync(dbContext, chat, participant.ParticipantId);
                }
            }
        }

        public async Task RemoveLocalModeratorFromChatsAsync(MainDbContext dbContext, ContestLocalModerator localModerator)
        {
            if (localModerator != null)
            {
                var contest = localModerator.Contest;

                var chats = await dbContext.Chats.Where(c => c.ContestId == contest.Id
                                                                && c.IsCreatedBySystem
                                                                && c.ChatUsers.Any(cu => cu.UserId == localModerator.LocalModeratorId))
                                                    .ToListAsync();

                foreach (var chat in chats)
                {
                    await _messenger.RemoveUserFromChatAsync(dbContext, chat, localModerator.LocalModeratorId);
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

            return chats.ConvertAll(c => _messenger.GetChatHistoryAsync(dbContext, c.Link, null, null, true).GetAwaiter().GetResult());
        }
    }
}
