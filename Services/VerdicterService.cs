using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Services
{
    public class VerdicterService
    {
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
                    points = solution.Points;
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
                default:
                    break;
            }
            return verdict;
        }

        public long GetAdditionalResultForSolutionSubmit(List<Solution> previousSolutions, Solution newSolution, RulesSet rules)
        {
            long result = 0;
            if (previousSolutions == null || newSolution == null || rules == null)
            {
                return result;
            }
            if (previousSolutions.Count == 0)
            {
                result = GetResultForSolution(newSolution, rules);
            }
            else
            {
                previousSolutions = previousSolutions.OrderBy(s => s.SubmitTimeUTC).ToList();
                switch (rules.CountMode)
                {
                    case RulesCountMode.CountPoints:
                        short newPoints = newSolution.Points;
                        short oldPoints;
                        if (rules.PointsForBestSolution)
                        {
                            oldPoints = previousSolutions.Max(s => s.Points);
                            result = newPoints > oldPoints ? newPoints - oldPoints : 0;
                        }
                        else
                        {
                            oldPoints = previousSolutions.Last().Points;
                            result = newPoints - oldPoints;
                        }
                        break;
                    case RulesCountMode.CountPenalty:
                        result = GetResultForSolution(newSolution, rules);
                        break;
                    case RulesCountMode.CountPointsMinusPenalty:
                        newPoints = newSolution.Points;
                        oldPoints = previousSolutions.Max(s => s.Points);
                        result = newPoints > oldPoints ? newPoints - oldPoints : 0;
                        result -= GetTimePenaltyForSolution(newSolution);
                        break;
                    default:
                        break;
                }
            }
            return result;
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
