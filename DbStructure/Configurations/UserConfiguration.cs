using ContestSystem.DbStructure.Models.Auth;
using ContestSystem.DbStructure.Models.Messenger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Many-to-Many к ролям
            builder.HasMany(u => u.Roles)
                    .WithMany(r => r.Users)
                    .UsingEntity<IdentityUserRole<long>>(
                        userRole => userRole.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId).IsRequired(),
                        userRole => userRole.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId).IsRequired()
                    );

            // Many-to-Many к чатам
            builder.HasMany(u => u.Chats)
                    .WithMany(c => c.Users)
                    .UsingEntity<ChatUser>(
                        cuLeftBuilder => cuLeftBuilder.HasOne(cu => cu.Chat)
                                                        .WithMany(c => c.ChatUsers)
                                                        .HasForeignKey(cu => cu.ChatId)
                                                        .IsRequired(),

                        cuRightBuilder => cuRightBuilder.HasOne(cu => cu.User)
                                                        .WithMany(u => u.ChatUsers)
                                                        .HasForeignKey(cu => cu.UserId)
                                                        .IsRequired()
                    );
        }
    }
}
