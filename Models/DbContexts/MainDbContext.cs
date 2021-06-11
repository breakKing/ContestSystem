using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContestSystem.Models.DbContexts
{
    public class MainDbContext : IdentityDbContext<User, Role, long>
    {
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePage> CoursesPages { get; set; }
        public DbSet<Example> Examples { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Checker> Checkers { get; set; }
        public DbSet<RulesSet> RulesSets { get; set; }
        public DbSet<ContestHistory> ContestsHistories { get; set; }
        public DbSet<VirtualContest> VirtualContests { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestsResults { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ContestProblem> ContestsProblems { get; set; }
        public DbSet<ContestParticipant> ContestsParticipants { get; set; }
        public DbSet<ContestLocalModerator> ContestsLocalModerators { get; set; }
        public DbSet<CourseLocalModerator> CoursesLocalModerators { get; set; }
        public DbSet<CourseParticipant> CoursesParticipants { get; set; }
        public DbSet<CourseProblem> CoursesProblems { get; set; }
        public DbSet<ContestLocalizer> ContestsLocalizers { get; set; }
        public DbSet<CourseLocalizer> CoursesLocalizers { get; set; }
        public DbSet<CoursePageLocalizer> CoursesPagesLocalizers { get; set; }
        public DbSet<ProblemLocalizer> ProblemsLocalizers { get; set; }
        public DbSet<PostLocalizer> PostsLocalizers { get; set; }
        public DbSet<ContestFile> ContestsFiles { get; set; }
        public DbSet<CoursePageFile> CoursesPagesFiles { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // связь ролей и пользователей
            builder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<IdentityUserRole<long>>(
                    userRole => userRole.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId).IsRequired(),
                    userRole => userRole.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId).IsRequired());

            
            // контесты - проблемы
            builder.Entity<ContestProblem>()
                .HasOne<Contest>(cp => cp.Contest)
                .WithMany(c => c.ContestProblems)
                .HasForeignKey(cp => cp.ContestId);
            
            builder.Entity<ContestProblem>()
                .HasOne<Problem>(cp => cp.Problem)
                .WithMany(c => c.ContestProblems)
                .HasForeignKey(cp => cp.ProblemId);
        }
    }
}