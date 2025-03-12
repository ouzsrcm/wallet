using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(3);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Symbol)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(x => x.Flag)
               .HasMaxLength(50);

        builder.Property(x => x.Format)
               .HasMaxLength(20);

        builder.Property(x => x.ExchangeRate)
               .HasColumnType("decimal(18,6)");

        // Add a unique index on the Code field
        builder.HasIndex(x => x.Code)
               .IsUnique();

        // Ensure only one currency can be default
        builder.HasIndex(x => x.IsDefault)
               .HasFilter("[IsDefault] = 1")
               .IsUnique();
    }
} 