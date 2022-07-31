using ContestSystem.DbStructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ContestConfiguration : IEntityTypeConfiguration<Contest>
    {
        public void Configure(EntityTypeBuilder<Contest> builder)
        {
            // Many-to-Many к задачам
            builder.HasMany(c => c.Problems)
                    .WithMany(p => p.Contests)
                    .UsingEntity<ContestProblem>(
                        cpLeftBuilder => cpLeftBuilder.HasOne(cp => cp.Problem)
                                                        .WithMany(p => p.ContestProblems)
                                                        .HasForeignKey(cp => cp.ProblemId)
                                                        .IsRequired(),

                        cpRightBuilder => cpRightBuilder.HasOne(cp => cp.Contest)
                                                        .WithMany(c => c.ContestProblems)
                                                        .HasForeignKey(cp => cp.ContestId)
                                                        .IsRequired()
                    );

            // Many-to-Many к участникам
            builder.HasMany(c => c.Participants)
                    .WithMany(p => p.ParticipatingContests)
                    .UsingEntity<ContestParticipant>(
                        cpLeftBuilder => cpLeftBuilder.HasOne(cp => cp.Participant)
                                                        .WithMany(p => p.ContestParticipants)
                                                        .HasForeignKey(cp => cp.ParticipantId)
                                                        .IsRequired(),

                        cpRightBuilder => cpRightBuilder.HasOne(cp => cp.Contest)
                                                        .WithMany(c => c.ContestParticipants)
                                                        .HasForeignKey(cp => cp.ContestId)
                                                        .IsRequired()
                    );

            // Many-to-Many к организаторам
            builder.HasMany(c => c.Organizers)
                    .WithMany(l => l.OrganizingContests)
                    .UsingEntity<ContestOrganizer>(
                        coLeftBuilder => coLeftBuilder.HasOne(co => co.Organizer)
                                                        .WithMany(l => l.ContestOrganizers)
                                                        .HasForeignKey(co => co.OrganizerId)
                                                        .IsRequired(),

                        coRightBuilder => coRightBuilder.HasOne(co => co.Contest)
                                                        .WithMany(c => c.ContestOrganizers)
                                                        .HasForeignKey(co => co.ContestId)
                                                        .IsRequired()
                    );

            // One-to-Many к создателям
            builder.HasOne(c => c.Creator)
                    .WithMany(u => u.CreatedContests)
                    .HasForeignKey(c => c.CreatorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many к модераторам
            builder.HasOne(c => c.ApprovingModerator)
                    .WithMany(u => u.ModeratedContests)
                    .HasForeignKey(c => c.ApprovingModeratorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
