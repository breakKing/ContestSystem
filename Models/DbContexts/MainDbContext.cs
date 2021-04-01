using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContestSystem.Models.DbContexts
{
    public class MainDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Example> Examples { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Checker> Checkers { get; set; }
        public DbSet<RulesSet> RulesSets { get; set; }
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
                .UsingEntity<IdentityUserRole<string>>(
                    userRole => userRole.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId).IsRequired(),
                    userRole => userRole.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId).IsRequired());
        }
    }
}