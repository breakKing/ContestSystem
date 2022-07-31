using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ContestProblemConfiguration : IEntityTypeConfiguration<ContestProblem>
    {
        public void Configure(EntityTypeBuilder<ContestProblem> builder)
        {
            builder.HasKey(cp => new { cp.ContestId, cp.ProblemId })
                    .HasName("PK_ContestsProblems")
                    .IsClustered();

            builder.HasIndex(cp => cp.ContestId);
        }
    }
}
