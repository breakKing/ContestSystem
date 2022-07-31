using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class CourseParticipantConfiguration : IEntityTypeConfiguration<CourseParticipant>
    {
        public void Configure(EntityTypeBuilder<CourseParticipant> builder)
        {
            builder.HasKey(cp => new { cp.CourseId, cp.ParticipantId })
                    .HasName("PK_CoursesParticipants")
                    .IsClustered();

            builder.HasIndex(cp => cp.CourseId);
        }
    }
}
