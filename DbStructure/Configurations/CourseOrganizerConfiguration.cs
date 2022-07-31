using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class CourseOrganizerConfiguration : IEntityTypeConfiguration<CourseOrganizer>
    {
        public void Configure(EntityTypeBuilder<CourseOrganizer> builder)
        {
            builder.HasKey(co => new { co.CourseId, co.OrganizerId })
                    .HasName("PK_CoursesOrganizers")
                    .IsClustered();

            builder.HasIndex(co => co.CourseId);
        }
    }
}
