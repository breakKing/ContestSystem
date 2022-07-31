using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Many-to-Many к задачам
            builder.HasMany(c => c.Problems)
                    .WithMany(p => p.Courses)
                    .UsingEntity<CourseProblem>(
                        cpLeftBuilder => cpLeftBuilder.HasOne(cp => cp.Problem)
                                                        .WithMany(p => p.CourseProblems)
                                                        .HasForeignKey(cp => cp.ProblemId)
                                                        .IsRequired(),

                        cpRightBuilder => cpRightBuilder.HasOne(cp => cp.Course)
                                                        .WithMany(c => c.CourseProblems)
                                                        .HasForeignKey(cp => cp.CourseId)
                                                        .IsRequired()
                    );

            // Many-to-Many к участникам
            builder.HasMany(c => c.Participants)
                    .WithMany(p => p.ParticipatingCourses)
                    .UsingEntity<CourseParticipant>(
                        cpLeftBuilder => cpLeftBuilder.HasOne(cp => cp.Participant)
                                                        .WithMany(p => p.CourseParticipants)
                                                        .HasForeignKey(cp => cp.ParticipantId)
                                                        .IsRequired(),

                        cpRightBuilder => cpRightBuilder.HasOne(cp => cp.Course)
                                                        .WithMany(c => c.CourseParticipants)
                                                        .HasForeignKey(cp => cp.CourseId)
                                                        .IsRequired()
                    );

            // Many-to-Many к организаторам
            builder.HasMany(c => c.Organizers)
                    .WithMany(l => l.OrganizingCourses)
                    .UsingEntity<CourseOrganizer>(
                        coLeftBuilder => coLeftBuilder.HasOne(co => co.Organizer)
                                                        .WithMany(l => l.CourseOrganizers)
                                                        .HasForeignKey(co => co.OrganizerId)
                                                        .IsRequired(),

                        coRightBuilder => coRightBuilder.HasOne(co => co.Course)
                                                        .WithMany(c => c.CourseOrganizers)
                                                        .HasForeignKey(co => co.CourseId)
                                                        .IsRequired()
                    );

            // One-to-Many к создателям
            builder.HasOne(c => c.Creator)
                    .WithMany(u => u.CreatedCourses)
                    .HasForeignKey(c => c.CreatorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many к модераторам
            builder.HasOne(c => c.ApprovingModerator)
                    .WithMany(u => u.ModeratedCourses)
                    .HasForeignKey(c => c.ApprovingModeratorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
