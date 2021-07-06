using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Services
{
    public class VerdicterService
    {
        public long GetResultForSolution(Solution solution)
        {
            long result = 0;
            if (solution == null)
            {
                return result;
            }
            switch (solution.Contest.RulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    result = SumPointsForAllTests(solution.TestResults);
                    break;
                case RulesCountMode.CountPenalty:
                    VerdictType verdict = GetVerdictForSolution(solution);
                    if (verdict != VerdictType.Accepted)
                    {
                        if (!(verdict == VerdictType.CompilationError && !solution.Contest.RulesSet.PenaltyForCompilationError))
                        {
                            result += solution.Contest.RulesSet.PenaltyForOneTry;
                        }
                    }
                    else
                    {
                        result += GetTimePenaltyForSolution(solution);
                    }
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    result = SumPointsForAllTests(solution.TestResults) - GetTimePenaltyForSolution(solution);
                    break;
                default:
                    break;
            }
            return result;
        }

        public VerdictType GetVerdictForSolution(Solution solution)
        {
            VerdictType verdict = VerdictType.Undefined;
            if (solution == null)
            {
                return verdict;
            }
            if (solution.TestResults == null || solution.TestResults.Count == 0)
            {
                return verdict;
            }
            switch (solution.Contest.RulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    long points = SumPointsForAllTests(solution.TestResults);
                    if (points == 0)
                    {
                        verdict = solution.TestResults.OrderBy(tr => tr.Number).Last().Verdict;
                    }
                    else if (points == 100)
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
                    long pts = SumPointsForAllTests(solution.TestResults);
                    if (pts == 0)
                    {
                        verdict = solution.TestResults.OrderBy(tr => tr.Number).Last().Verdict;
                    }
                    else if (pts == 100)
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

        public long GetResultForProblem(List<Solution> solutions)
        {
            long result = 0;
            if (solutions == null || solutions.Count == 0)
            {
                return result;
            }
            solutions = solutions.OrderBy(s => s.SubmitTimeUTC).ToList();
            switch (solutions[0].Contest.RulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    if (solutions[0].Contest.RulesSet.PointsForBestSolution)
                    {
                        result = solutions.Max(s => s.Points);
                    }
                    else
                    {
                        result = solutions.Last().Points;
                    }
                    break;
                case RulesCountMode.CountPenalty:
                    if (solutions.Any(s => GetVerdictForSolution(s) == VerdictType.Accepted))
                    {
                        result = 100;
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    result = solutions.Select(s => GetTimePenaltyForSolution(s)).Sum() + solutions.Max(s => SumPointsForAllTests(s.TestResults));
                    break;
                default:
                    break;
            }
            return result;
        }

        public VerdictType GetVerdictForProblem(List<Solution> solutions)
        {
            VerdictType verdict = VerdictType.Undefined;
            if (solutions == null || solutions.Count == 0)
            {
                return verdict;
            }
            solutions = solutions.OrderBy(s => s.SubmitTimeUTC).ToList();
            switch (solutions[0].Contest.RulesSet.CountMode)
            {
                case RulesCountMode.CountPoints:
                    if (solutions.Any(s => SumPointsForAllTests(s.TestResults) == 100))
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else if (solutions.Any(s => SumPointsForAllTests(s.TestResults) > 0))
                    {
                        verdict = VerdictType.PartialSolution;
                    }
                    else
                    {
                        verdict = VerdictType.WrongAnswer;
                    }
                    break;
                case RulesCountMode.CountPenalty:
                    if (solutions.Any(s => AllTestsAreAccepted(s.TestResults)))
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else
                    {
                        verdict = VerdictType.WrongAnswer;
                    }
                    break;
                case RulesCountMode.CountPointsMinusPenalty:
                    if (solutions.Any(s => SumPointsForAllTests(s.TestResults) == 100))
                    {
                        verdict = VerdictType.Accepted;
                    }
                    else if (solutions.Any(s => SumPointsForAllTests(s.TestResults) > 0))
                    {
                        verdict = VerdictType.PartialSolution;
                    }
                    else
                    {
                        verdict = VerdictType.WrongAnswer;
                    }
                    break;
                default:
                    break;
            }
            return verdict;
        }

        public short SumPointsForAllTests(List<TestResult> testResults)
        {
            if (testResults == null || testResults.Count == 0)
            {
                return 0;
            }
            return (short)testResults.Sum(tr => tr.GotPoints);
        }

        private static long GetTimePenaltyForSolution(Solution solution)
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

        private static bool AllTestsAreAccepted(List<TestResult> testResults)
        {
            if (testResults == null || testResults.Count == 0)
            {
                return false;
            }
            return testResults.TrueForAll(tr => tr.Verdict == VerdictType.Accepted);
        }
    }
}
