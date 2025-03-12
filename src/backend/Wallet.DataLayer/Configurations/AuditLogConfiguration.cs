using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ActionType)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.UserId)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.UserName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ActionDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.OldValues)
               .HasMaxLength(4000);

        builder.Property(x => x.NewValues)
               .HasMaxLength(4000);

        builder.Property(x => x.AffectedColumns)
               .HasMaxLength(1000);

        builder.Property(x => x.PrimaryKey)
               .HasMaxLength(100);

        builder.Property(x => x.TableName)
               .HasMaxLength(100);

        builder.Property(x => x.IPAddress)
               .HasMaxLength(50);

        builder.Property(x => x.UserAgent)
               .HasMaxLength(500);

        builder.Property(x => x.RequestUrl)
               .HasMaxLength(500);

        builder.Property(x => x.RequestMethod)
               .HasMaxLength(20);

        builder.Property(x => x.RequestBody)
               .HasMaxLength(4000);

        builder.Property(x => x.ErrorMessage)
               .HasMaxLength(4000);

        builder.Property(x => x.StackTrace)
               .HasMaxLength(4000);

        builder.Property(x => x.AdditionalInfo)
               .HasMaxLength(4000);

        // İndeksler
        builder.HasIndex(x => x.ActionDate);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EntityName);
        builder.HasIndex(x => x.ActionType);
    }
} 