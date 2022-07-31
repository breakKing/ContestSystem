using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ExampleConfiguration : IEntityTypeConfiguration<Example>
    {
        public void Configure(EntityTypeBuilder<Example> builder)
        {
            builder.HasKey(e => new { e.ProblemId, e.Number })
                    .HasName("PK_Examples")
                    .IsClustered();

            builder.HasIndex(e => e.ProblemId);
        }
    }
}
