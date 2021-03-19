﻿using ContestSystem.Models.Constants;
using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class CompetitionMonitorEntryOutputModel : IOutputModel<ContestsParticipantsBaseModel>
    {
        private readonly ContestSystemDbContext _dbContext;

        public string Username { get; set; }
        public int CompetitionMode { get; set; }
        public List<bool> ProblemSolved { get; set; } = new List<bool>();
        public List<short> ScoreForProblem { get; set; } = new List<short>();
        public List<short> ProblemTries { get; set; } = new List<short>();
        public List<string> LastSolutionDateTime { get; set; } = new List<string>();

        public CompetitionMonitorEntryOutputModel(ContestSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void TransformForOutput(ContestsParticipantsBaseModel baseModel)
        {
            Username = baseModel.Alias;
            CompetitionMode = (int)baseModel.Contest.Mode;

            List<ProblemBaseModel> problems = _dbContext.ContestsProblems.Where(cp => cp.ContestId == baseModel.ContestId)
                                                                            .OrderBy(cp => cp.Alias)
                                                                            .Select(cp => cp.Problem)
                                                                            .ToList();

            List<SolutionBaseModel> solutions = _dbContext.Solutions.Where(sol => sol.ParticipantId == baseModel.ParticipantId 
                                                                                    && sol.ContestId == baseModel.ContestId)
                                                                    .ToList();

            DateTime currentDateUTC = DateTime.UtcNow;
            foreach (ProblemBaseModel problem in problems)
            {
                if (baseModel.Contest.Mode == ContestMode.CountPoints)
                {
                    ScoreForProblem.Add(solutions.Where(sol => sol.ProblemId == problem.Id 
                                                                && sol.Verdict != VerdictType.TestInProgress 
                                                                && sol.Verdict != VerdictType.Undefined 
                                                                && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                    .Select(sol => sol.Points)
                                                    .Max());
                }
                if (baseModel.Contest.Mode == ContestMode.CountSolvedProblemsAndPenalty)
                {
                    ProblemSolved.Add(solutions.Any(sol => sol.ProblemId == problem.Id 
                                                        && sol.Verdict == VerdictType.Accepted 
                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC)));

                    ProblemTries.Add((short)solutions.Where(sol => sol.ProblemId == problem.Id
                                                                    && sol.Verdict != VerdictType.TestInProgress
                                                                    && sol.Verdict != VerdictType.Undefined
                                                                    && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                        .Count());

                    if (ProblemSolved.Last())
                    {
                        LastSolutionDateTime.Add(solutions.Where(sol => sol.ProblemId == problem.Id
                                                                        && sol.Verdict == VerdictType.Accepted
                                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                            .Select(sol => sol.SubmitTimeUTC - baseModel.Contest.StartDateTimeUTC)
                                                            .Max()
                                                            .ToString(@"hh\:mm"));
                    }
                    else
                    {
                        LastSolutionDateTime.Add(solutions.Where(sol => sol.ProblemId == problem.Id
                                                                        && sol.Verdict != VerdictType.TestInProgress
                                                                        && sol.Verdict != VerdictType.Undefined
                                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                            .Select(sol => sol.SubmitTimeUTC - baseModel.Contest.StartDateTimeUTC)
                                                            .Max()
                                                            .ToString(@"hh\:mm"));
                    }
                }
            }
        }

        public async Task TransformForOutputAsync(ContestsParticipantsBaseModel baseModel)
        {
            Username = baseModel.Alias;
            CompetitionMode = (int)baseModel.Contest.Mode;

            List<ProblemBaseModel> problems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == baseModel.ContestId)
                                                                                .OrderBy(cp => cp.Alias)
                                                                                .Select(cp => cp.Problem)
                                                                                .ToListAsync();

            List<SolutionBaseModel> solutions = await _dbContext.Solutions.Where(sol => sol.ParticipantId == baseModel.ParticipantId
                                                                                    && sol.ContestId == baseModel.ContestId)
                                                                            .ToListAsync();

            DateTime currentDateUTC = DateTime.UtcNow;
            foreach (ProblemBaseModel problem in problems)
            {
                if (baseModel.Contest.Mode == ContestMode.CountPoints)
                {
                    ScoreForProblem.Add(solutions.Where(sol => sol.ProblemId == problem.Id
                                                                && sol.Verdict != VerdictType.TestInProgress
                                                                && sol.Verdict != VerdictType.Undefined
                                                                && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                    .Select(sol => sol.Points)
                                                    .Max());
                }
                if (baseModel.Contest.Mode == ContestMode.CountSolvedProblemsAndPenalty)
                {
                    ProblemSolved.Add(solutions.Any(sol => sol.ProblemId == problem.Id
                                                        && sol.Verdict == VerdictType.Accepted
                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC)));

                    ProblemTries.Add((short)solutions.Where(sol => sol.ProblemId == problem.Id
                                                                    && sol.Verdict != VerdictType.TestInProgress
                                                                    && sol.Verdict != VerdictType.Undefined
                                                                    && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                        .Count());

                    if (ProblemSolved.Last())
                    {
                        LastSolutionDateTime.Add(solutions.Where(sol => sol.ProblemId == problem.Id
                                                                        && sol.Verdict == VerdictType.Accepted
                                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                            .Select(sol => sol.SubmitTimeUTC - baseModel.Contest.StartDateTimeUTC)
                                                            .Max()
                                                            .ToString(@"hh\:mm"));
                    }
                    else
                    {
                        LastSolutionDateTime.Add(solutions.Where(sol => sol.ProblemId == problem.Id
                                                                        && sol.Verdict != VerdictType.TestInProgress
                                                                        && sol.Verdict != VerdictType.Undefined
                                                                        && IsBeforeFreeze(baseModel.Contest, sol, currentDateUTC))
                                                            .Select(sol => sol.SubmitTimeUTC - baseModel.Contest.StartDateTimeUTC)
                                                            .Max()
                                                            .ToString(@"hh\:mm"));
                    }
                }
            }
        }

        private static bool IsBeforeFreeze(ContestBaseModel contest, SolutionBaseModel solution, DateTime curDateTimeUTC)
        {
            return solution.SubmitTimeUTC.AddMinutes(Math.Max(contest.DurationInMinutes - SystemConstants.freezeWhenMinutesBeforeFinish, 0)) >= curDateTimeUTC;
        }
    }
}
