using ContestSystem.DbStructure.Configurations;
using ContestSystem.DbStructure.Models;
using ContestSystem.DbStructure.Models.Auth;
using ContestSystem.DbStructure.Models.Messenger;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ContestSystem.Models.DbContexts
{
    public class MainDbContext : IdentityDbContext<User, Role, long>
    {
        public DbSet<Session> Sessions { get; set; }
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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ContestProblem> ContestsProblems { get; set; }
        public DbSet<ContestParticipant> ContestsParticipants { get; set; }
        public DbSet<ContestOrganizer> ContestsOrganizers { get; set; }
        public DbSet<CourseOrganizer> CoursesOrganizers { get; set; }
        public DbSet<CourseParticipant> CoursesParticipants { get; set; }
        public DbSet<CourseProblem> CoursesProblems { get; set; }
        public DbSet<ContestLocalizer> ContestsLocalizers { get; set; }
        public DbSet<CourseLocalizer> CoursesLocalizers { get; set; }
        public DbSet<CoursePageLocalizer> CoursesPagesLocalizers { get; set; }
        public DbSet<ProblemLocalizer> ProblemsLocalizers { get; set; }
        public DbSet<PostLocalizer> PostsLocalizers { get; set; }
        public DbSet<ContestFile> ContestsFiles { get; set; }
        public DbSet<CoursePageFile> CoursesPagesFiles { get; set; }
        public DbSet<CheckerServer> CheckerServers { get; set; }
        public DbSet<CheckerServerCompiler> CheckerServersCompilers { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatsUsers { get; set; }
        public DbSet<ChatEvent> ChatsEvents { get; set; }
        public DbSet<ChatMessage> ChatsMessages { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Конфигурация обычных сущностей через Fluent API
            builder.ApplyConfiguration(new ChatConfiguration());
            builder.ApplyConfiguration(new ContestConfiguration());
            builder.ApplyConfiguration(new CourseConfiguration());
            builder.ApplyConfiguration(new ExampleConfiguration());
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new TestConfiguration());
            builder.ApplyConfiguration(new TestResultConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());

            // Конфигурация сущностей Many-to-Many через Fluent API
            builder.ApplyConfiguration(new ChatUserConfiguration());
            builder.ApplyConfiguration(new ContestOrganizerConfiguration());
            builder.ApplyConfiguration(new ContestParticipantConfiguration());
            builder.ApplyConfiguration(new ContestProblemConfiguration());
            builder.ApplyConfiguration(new CourseOrganizerConfiguration());
            builder.ApplyConfiguration(new CourseParticipantConfiguration());
            builder.ApplyConfiguration(new CourseProblemConfiguration());
        }

        public async Task<bool> SecureSaveAsync()
        {
            try
            {
                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }
    }
}