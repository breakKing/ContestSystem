using ContestSystem.DbStructure.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => new { s.UserId, s.Fingerprint })
                    .HasName("PK_Sessions")
                    .IsClustered();

            builder.HasIndex(s => s.UserId);
        }
    }
}
