using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ContestOrganizerConfiguration : IEntityTypeConfiguration<ContestOrganizer>
    {
        public void Configure(EntityTypeBuilder<ContestOrganizer> builder)
        {
            builder.HasKey(co => new { co.ContestId, co.OrganizerId })
                    .HasName("PK_ContestsOrganizers")
                    .IsClustered();

            builder.HasIndex(co => co.ContestId);
        }
    }
}
