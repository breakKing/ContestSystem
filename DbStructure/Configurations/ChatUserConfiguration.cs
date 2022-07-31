using ContestSystem.DbStructure.Models.Messenger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
    {
        public void Configure(EntityTypeBuilder<ChatUser> builder)
        {
            builder.HasKey(co => new { co.ChatId, co.UserId })
                    .HasName("PK_ChatsUsers")
                    .IsClustered();

            builder.HasIndex(co => co.ChatId);
        }
    }
}
