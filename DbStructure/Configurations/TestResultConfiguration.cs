using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.HasKey(tr => new { tr.SolutionId, tr.Number })
                    .HasName("PK_TestsResults")
                    .IsClustered();

            builder.HasIndex(tr => tr.SolutionId);
        }
    }
}
