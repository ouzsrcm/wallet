using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Email)
               .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(x => x.PasswordHash)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.PasswordSalt)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.TwoFactorType)
               .HasMaxLength(20);

        builder.Property(x => x.TwoFactorKey)
               .HasMaxLength(100);

        builder.Property(x => x.SecurityStamp)
               .HasMaxLength(100);

        builder.Property(x => x.LastLoginIP)
               .HasMaxLength(50);

        builder.Property(x => x.LastFailedLoginIP)
               .HasMaxLength(50);

        builder.Property(x => x.RefreshToken)
               .HasMaxLength(100);

        builder.Property(x => x.PasswordResetToken)
               .HasMaxLength(100);

        builder.Property(x => x.EmailVerificationToken)
               .HasMaxLength(100);

        builder.Property(x => x.PhoneVerificationToken)
               .HasMaxLength(100);

        builder.Property(x => x.DeviceId)
               .HasMaxLength(100);

        builder.Property(x => x.DeviceName)
               .HasMaxLength(100);

        builder.Property(x => x.Roles)
               .HasConversion(
                   v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null!),
                   v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null!) ?? new List<string>()
               );

        // Unique indexes
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
        builder.HasIndex(x => x.PhoneNumber).IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");
    }
} 