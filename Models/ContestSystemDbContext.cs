using CheckerSystemBaseStructures;
using ContestSystem.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models
{
    public class ContestSystemDbContext: IdentityDbContext<UserBaseModel>
    {
        public DbSet<ContestBaseModel> Contests { get; set; }
        public DbSet<ExampleBaseModel> Examples { get; set; }
        public DbSet<ProblemBaseModel> Problems { get; set; }
        public DbSet<SolutionBaseModel> Solutions { get; set; }
        public DbSet<TestBaseModel> Tests { get; set; }
        public DbSet<MessageBaseModel> Messages { get; set; }
        public DbSet<PostBaseModel> Posts { get; set; }
        public DbSet<ContestsProblemsBaseModel> ContestsProblems { get; set; }
        public DbSet<ContestsParticipantsBaseModel> ContestsParticipants { get; set; }
        public DbSet<ContestsModerators> ContestsModerators { get; set; }

        public ContestSystemDbContext(DbContextOptions<ContestSystemDbContext> options): base(options)
        {
            Database.EnsureCreated();
            DefaultInit();  
        }

        public void DefaultInit()
        {
            if (!Contests.Any())
            {
                Contests.Add
                (
                    new ContestBaseModel
                    {
                        Name = "Пробное публичное соревнование",
                        Description = "Соревнование для проверки работоспособности системы",
                        IsPublic = true,
                        IsForever = true,
                        StartDateTime = DateTime.Now.AddDays(-2),
                        DurationInMinutes = 0
                    }
                );
                SaveChanges();
            }
            if (!Problems.Any())
            {
                Problems.Add
                (
                    new ProblemBaseModel
                    {
                        Name = "A + B",
                        Description = "Дано два целых числа A и B. Необходимо найти их сумму.",
                        InputBlock = "На вход даётся два целых числа A и B (от -10^9 до 10^9), разделённые пробелом.",
                        OutputBlock = "Необходимо вывести одно целое число - сумму чисел A и B.",
                        IsPublic = true,
                        TimeLimit = 1000,
                        MemoryLimit = 16,
                        Type = ProblemType.FullSolution
                    }
                );
                SaveChanges();
            }
            if (!Examples.Any())
            {
                Examples.AddRange
                (
                    new ExampleBaseModel
                    {
                        Number = 1,
                        InputText = "4 5",
                        OutputText = "9",
                        Problem = Problems.FirstOrDefault(problem => problem.Id == 1)
                    },
                    new ExampleBaseModel
                    {
                        Number = 2,
                        InputText = "-1 181",
                        OutputText = "180",
                        Problem = Problems.FirstOrDefault(problem => problem.Id == 1)
                    }
                );
                SaveChanges();
            }
            if (!ContestsProblems.Any())
            {
                ContestsProblems.Add
                (
                    new ContestsProblemsBaseModel
                    {
                        Contest = Contests.FirstOrDefault(contest => contest.Id == 1),
                        Problem = Problems.FirstOrDefault(problem => problem.Id == 1),
                        Alias = 'A'
                    }
                );
                SaveChanges();
            }
        }
    }
}
