using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.HasKey(t => new { t.ProblemId, t.Number })
                    .HasName("PK_Tests")
                    .IsClustered();

            builder.HasIndex(t => t.ProblemId);
        }
    }
}
