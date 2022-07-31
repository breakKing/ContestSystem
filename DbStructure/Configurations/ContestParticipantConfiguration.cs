using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ContestParticipantConfiguration : IEntityTypeConfiguration<ContestParticipant>
    {
        public void Configure(EntityTypeBuilder<ContestParticipant> builder)
        {
            builder.HasKey(cp => new { cp.ContestId, cp.ParticipantId })
                    .HasName("PK_ContestsParticipants")
                    .IsClustered();

            builder.HasIndex(cp => cp.ContestId);
        }
    }
}
