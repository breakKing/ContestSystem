using ContestSystem.Areas.Solutions.Services;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
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

        public ContestsManagerService(SolutionsManagerService solutionsManager)
        {
            _solutionsManager = solutionsManager;
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
    }
}
