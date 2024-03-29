﻿using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using ContestSystem.DbStructure.Enums;
using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Solutions.Services
{
    public class SolutionsManagerService
    {
        private readonly CheckerSystemService _checkerSystem;
        private readonly NotifierService _notifier;

        public SolutionsManagerService(CheckerSystemService checkerSystem, NotifierService notifier)
        {
            _checkerSystem = checkerSystem;
            _notifier = notifier;
        }

        public async Task<List<CompilerInfo>> GetCompilersAsync(MainDbContext dbContext)
        {
            return await _checkerSystem.GetAvailableCompilersAsync(dbContext);
        }

        public async Task<FormCheckStatus> CheckSolutionFormAsync(MainDbContext dbContext, SolutionForm form)
        {
            var status = FormCheckStatus.Undefined;

            var now = DateTime.UtcNow;

            bool isContestExistent = await dbContext.Contests.AnyAsync(c => c.Id == form.ContestId
                                                                            && c.ApprovalStatus == ApproveType.Accepted
                                                                            && c.StartDateTimeUTC <= now
                                                                            && c.StartDateTimeUTC.AddMinutes(c.DurationInMinutes) > now);

            if (isContestExistent)
            {
                bool isUserParticipant = await dbContext.ContestsParticipants.AnyAsync(cp => cp.ContestId == form.ContestId
                                                                                            && cp.ParticipantId == form.UserId
                                                                                            && cp.ConfirmedByOrganizer
                                                                                            && cp.ConfirmedByParticipant);

                if (isUserParticipant)
                {
                    bool isProblemExistent = await dbContext.ContestsProblems.AnyAsync(cp => cp.ContestId == form.ContestId
                                                                                        && cp.ProblemId == form.ProblemId);

                    if (isProblemExistent)
                    {
                        var compilerInfo = await GetCompilersAsync(dbContext);
                        bool isCompilerExistent = (compilerInfo != null && compilerInfo.Count != 0 && compilerInfo.Any(c => c.GUID == form.CompilerGUID));

                        if (isCompilerExistent)
                        {
                            bool isSolutionExistent = await dbContext.Solutions.AnyAsync(s => s.ContestId == form.ContestId
                                                                                                && s.ParticipantId == form.UserId
                                                                                                && s.ProblemId == form.ProblemId
                                                                                                && s.CompilerGUID == form.CompilerGUID
                                                                                                && s.Code == form.Code);

                            if (isSolutionExistent)
                            {
                                status = FormCheckStatus.ExistentSolution;
                            }
                            else
                            {
                                var contestProblem = await dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ContestId == form.ContestId
                                                                                                            && cp.ProblemId == form.ProblemId);
                                var solutionsCount = await CountUserSolutionsForProblemAsync(dbContext, contestProblem.Contest, contestProblem.Problem, form.UserId);

                                bool isLimitExceeded = solutionsCount >= contestProblem.Contest.RulesSet.MaxTriesForOneProblem;
                                if (isLimitExceeded)
                                {
                                    status = FormCheckStatus.LimitExceeded;
                                }
                                else
                                {
                                    status = FormCheckStatus.Correct;
                                }
                            }
                        }
                        else
                        {
                            status = FormCheckStatus.NonExistentCompiler;
                        }
                    }
                    else
                    {
                        status = FormCheckStatus.NonExistentProblem;
                    }
                }
                else
                {
                    status = FormCheckStatus.NonExistentParticipant;
                }
            }
            else
            {
                status = FormCheckStatus.NonExistentContest;
            }

            return status;
        }

        public async Task<CreationStatusData<long?>> CreateSolutionAsync(MainDbContext dbContext, SolutionForm form)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            var solution = new Solution
            {
                Code = form.Code,
                CompilerGUID = form.CompilerGUID,
                ContestId = form.ContestId,
                ParticipantId = form.UserId,
                ProblemId = form.ProblemId,
                SubmitTimeUTC = DateTime.UtcNow,
                CompilerName = form.CompilerName,
                ErrorsMessage = "",
                Verdict = VerdictType.Undefined,
                Points = 0
            };

            await dbContext.Solutions.AddAsync(solution);

            bool saveSuccess = await dbContext.SecureSaveAsync();
            if (!saveSuccess)
            {
                statusData.Status = CreationStatus.DbSaveError;
            }
            else
            {
                statusData.Status = CreationStatus.Success;
                statusData.Id = solution.Id;
            }

            return statusData;
        }

        public async Task<Solution> CompileSolutionAsync(MainDbContext dbContext, Solution solution)
        {
            if (solution != null)
            {
                var newSolution = await _checkerSystem.CompileSolutionAsync(dbContext, solution);
                if (newSolution == null)
                {
                    solution.Verdict = VerdictType.CheckerServersUnavailable;
                }
                else
                {
                    solution.ErrorsMessage = newSolution.ErrorsMessage;
                    solution.Verdict = newSolution.Verdict;
                }

                dbContext.Solutions.Update(solution);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solution.Id);
                    solution = await CompileSolutionAsync(dbContext, solution);
                }
            }
            return solution;
        }

        public async Task<Solution> TestSolutionAsync(MainDbContext dbContext, Solution solution)
        {
            if (solution != null)
            {
                bool state = true;
                List<Test> tests = solution.Problem.Tests.OrderBy(t => t.Number).ToList();
                foreach (var test in tests)
                {
                    var result = await RunTestAsync(dbContext, solution, test);
                    if (result == null)
                    {
                        state = false;
                        break;
                    }
                    if (result.Verdict != VerdictType.Accepted && solution.Contest.RulesSet.CountMode == RulesCountMode.CountPenalty)
                    {
                        break;
                    }
                }
                if (state)
                {
                    solution.Verdict = GetVerdictForSolution(solution, solution.Contest.RulesSet);
                    dbContext.Solutions.Update(solution);

                    bool saveSuccess = false;
                    while (!saveSuccess)
                    {
                        var contestParticipant = await dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ParticipantId == solution.ParticipantId && cp.ContestId == solution.ContestId);
                        var otherSolutions = await dbContext.Solutions.Where(s => s.ParticipantId == solution.ParticipantId
                                                                                    && s.ContestId == solution.ContestId
                                                                                    && s.ProblemId == solution.ProblemId
                                                                                    && s.Id != solution.Id)
                                                                        .ToListAsync();

                        dbContext.ContestsParticipants.Update(contestParticipant);

                        saveSuccess = await dbContext.SecureSaveAsync();
                    }
                }
            }
            return solution;
        }

        private async Task<TestResult> RunTestAsync(MainDbContext dbContext, Solution solution, Test test)
        {
            var testResult = solution.TestResults.FirstOrDefault(tr => tr.Number == test.Number);

            if (testResult == null)
            {
                var newTestResult = await _checkerSystem.RunTestForSolutionAsync(dbContext, solution, test.Number);
                if (newTestResult == null)
                {
                    solution.Verdict = VerdictType.CheckerServersUnavailable;
                }
                else
                {
                    solution.Points += newTestResult.GotPoints;
                    solution.TestResults.Add(newTestResult);
                }
                dbContext.Solutions.Update(solution);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                while (!saveSuccess)
                {
                    solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == solution.Id);
                    newTestResult = await RunTestAsync(dbContext, solution, test);
                }
                await _notifier.UpdateOnSolutionActualResultAsync(solution.Contest, solution);
                return newTestResult;
            }

            return testResult;
        }

        public long GetResultForSolution(Solution solution, RulesSet rules)
        {
            long result = 0;
            if (solution == null || rules == null)
            {
                return result;
            }
            switch (rules.CountMode)
            {
                case RulesCountMode.CountPoints:
                    result = solution.Points;
                    break;
                case RulesCountMode.CountPenalty:
                    VerdictType verdict = GetVerdictForSolution(solution, rules);
                    if (verdict != VerdictType.Accepted)
                    {
                        if (!(verdict == VerdictType.CompilationError && !rules.PenaltyForCompilationError))
                        {
                            result += rules.PenaltyForOneTry;
                        }
                    }
                    else
                    {
                        result += GetTimePenaltyForSolution(solution);
                    }
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    result = solution.Points - GetTimePenaltyForSolution(solution);
                    break;
                default:
                    break;
            }
            return result;
        }

        public VerdictType GetVerdictForSolution(Solution solution, RulesSet rules)
        {
            VerdictType verdict = VerdictType.Undefined;
            if (solution == null || rules == null)
            {
                return verdict;
            }
            if (solution.TestResults == null || solution.TestResults.Count == 0)
            {
                return verdict;
            }
            switch (rules.CountMode)
            {
                case RulesCountMode.CountPoints:
                    short points = solution.Points;
                    if (points == 0)
                    {
                        verdict = solution.TestResults.OrderBy(tr => tr.Number).Last().Verdict;
                    }
                    else if (points == Constants.MaxPointsSumForAllTests)
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else
                    {
                        verdict = VerdictType.PartialSolution;
                    }
                    break;
                case RulesCountMode.CountPenalty:
                    if (AllTestsAreAccepted(solution.TestResults))
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else
                    {
                        verdict = solution.TestResults.OrderBy(tr => tr.Number).Last().Verdict;
                    }
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    points = solution.Points;
                    if (points == 0)
                    {
                        verdict = solution.TestResults.OrderBy(tr => tr.Number).Last().Verdict;
                    }
                    else if (points == Constants.MaxPointsSumForAllTests)
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else
                    {
                        verdict = VerdictType.PartialSolution;
                    }
                    break;
                default:
                    break;
            }
            return verdict;
        }

        public async Task<int> CountUserSolutionsForProblemAsync(MainDbContext dbContext, Contest contest, Problem problem, long userId)
        {
            if (contest == null || problem == null)
            {
                return 0;
            }

            var rulesSet = contest.RulesSet;
            bool penaltyForCompilationError = rulesSet.CountMode == RulesCountMode.CountPenalty
                                                && rulesSet.PenaltyForCompilationError;

            return await dbContext.Solutions.CountAsync(s => s.ContestId == contest.Id
                                                                && s.ParticipantId == userId
                                                                && s.ProblemId == problem.Id
                                                                && (!penaltyForCompilationError || s.Verdict != VerdictType.CompilationError));
        }

        public long GetResultForAllSolutions(List<Solution> solutions, Contest contest)
        {
            long result = 0;

            if (solutions == null || solutions.Count == 0 || contest == null)
            {
                return result;
            }

            contest.ContestProblems.ForEach(cp =>
            {
                var solutionsForProblem = solutions.Where(s => s.ProblemId == cp.ProblemId).ToList();
                result += GetResultForProblem(solutionsForProblem, contest.RulesSet);
            });

            return result;
        }

        public short GetPointsForProblem(List<Solution> solutions, RulesSet rulesSet)
        {
            short points = 0;
            if (solutions == null || solutions.Count == 0 || rulesSet == null)
            {
                return points;
            }

            solutions = solutions.OrderBy(s => s.SubmitTimeUTC).ToList();
            switch (rulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    if (rulesSet.PointsForBestSolution)
                    {
                        points = solutions.Max(s => s.Points);
                    }
                    else
                    {
                        points = solutions.Last().Points;
                    }
                    break;
                case RulesCountMode.CountPenalty:
                    points = (short)(solutions.Any(s => GetVerdictForSolution(s, rulesSet) == VerdictType.Accepted) ? 100 : 0);
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    points = solutions.Max(s => s.Points);
                    break;
                default:
                    break;
            }

            return points;
        }

        private long GetResultForProblem(List<Solution> solutions, RulesSet rulesSet)
        {
            long result = 0;
            if (solutions == null || solutions.Count == 0 || rulesSet == null)
            {
                return result;
            }
            solutions = solutions.OrderBy(s => s.SubmitTimeUTC).ToList();
            switch (rulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    if (rulesSet.PointsForBestSolution)
                    {
                        result = solutions.Max(s => s.Points);
                    }
                    else
                    {
                        result = solutions.Last().Points;
                    }
                    break;
                case RulesCountMode.CountPenalty:
                    solutions.ForEach(s => result += GetResultForSolution(s, rulesSet));
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    result = -solutions.Select(s => GetTimePenaltyForSolution(s)).Sum() + solutions.Max(s => s.Points);
                    break;
                default:
                    break;
            }
            return result;
        }

        private long GetTimePenaltyForSolution(Solution solution)
        {
            long penalty = 0;
            if (solution == null)
            {
                return penalty;
            }
            short minutes = (short)(solution.SubmitTimeUTC - solution.Contest.StartDateTimeUTC).TotalMinutes;
            penalty = minutes * solution.Contest.RulesSet.PenaltyForOneMinute;
            return penalty;
        }

        private bool AllTestsAreAccepted(List<TestResult> testResults)
        {
            if (testResults == null || testResults.Count == 0)
            {
                return false;
            }
            return testResults.TrueForAll(tr => tr.Verdict == VerdictType.Accepted);
        }
    }
}
