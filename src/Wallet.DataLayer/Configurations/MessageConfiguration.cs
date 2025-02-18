using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Content)
               .IsRequired()
               .HasMaxLength(4000);

        // İlişkiler
        builder.HasOne(x => x.Sender)
               .WithMany()
               .HasForeignKey(x => x.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
               .WithMany()
               .HasForeignKey(x => x.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentMessage)
               .WithMany(x => x.Replies)
               .HasForeignKey(x => x.ParentMessageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
} 