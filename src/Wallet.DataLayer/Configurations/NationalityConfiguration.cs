using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class NationalityConfiguration : IEntityTypeConfiguration<Nationality>
{
    public void Configure(EntityTypeBuilder<Nationality> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(2);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.NativeName)
               .HasMaxLength(100);

        builder.Property(x => x.Alpha3Code)
               .HasMaxLength(3);

        builder.Property(x => x.NumericCode)
               .HasMaxLength(3);

        builder.Property(x => x.PhoneCode)
               .HasMaxLength(5);

        builder.Property(x => x.Capital)
               .HasMaxLength(100);

        builder.Property(x => x.Region)
               .HasMaxLength(50);

        builder.Property(x => x.SubRegion)
               .HasMaxLength(50);

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Alpha3Code).IsUnique();
        builder.HasIndex(x => x.NumericCode).IsUnique();
    }
} 