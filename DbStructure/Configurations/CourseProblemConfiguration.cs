using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class CourseProblemConfiguration : IEntityTypeConfiguration<CourseProblem>
    {
        public void Configure(EntityTypeBuilder<CourseProblem> builder)
        {
            builder.HasKey(cp => new { cp.CourseId, cp.ProblemId })
                    .HasName("PK_CoursesProblems")
                    .IsClustered();

            builder.HasIndex(cp => cp.CourseId);
        }
    }
}
