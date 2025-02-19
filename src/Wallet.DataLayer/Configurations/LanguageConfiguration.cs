using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NativeName)
            .HasMaxLength(100);

        builder.Property(x => x.FlagUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LocalizationCode)
            .HasMaxLength(20);

        builder.Property(x => x.DateFormat)
            .HasMaxLength(50);

        builder.Property(x => x.TimeFormat)
            .HasMaxLength(50);

        builder.Property(x => x.CurrencyFormat)
            .HasMaxLength(50);

        builder.Property(x => x.NumberFormat)
            .HasMaxLength(50);

        // Unique constraint for Code
        builder.HasIndex(x => x.Code).IsUnique();

        // Only one default language
        builder.HasIndex(x => x.IsDefault)
            .HasFilter("[IsDefault] = 1")
            .IsUnique();
    }
} 