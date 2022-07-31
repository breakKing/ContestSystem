using ContestSystem.DbStructure.Models.Messenger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContestSystem.DbStructure.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            // One-to-many к админам чатов
            builder.HasOne(c => c.Admin)
                    .WithMany(u => u.AdminingChats)
                    .HasForeignKey(c => c.AdminId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
