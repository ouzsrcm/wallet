using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;
using Wallet.Entities.Enums;

namespace Wallet.DataLayer.Configurations;

public class PersonLanguageConfiguration : IEntityTypeConfiguration<PersonLanguage>
{
    public void Configure(EntityTypeBuilder<PersonLanguage> builder)
    {
        builder.HasKey(pl => new { pl.PersonId, pl.LanguageId });

        builder.HasOne(pl => pl.Person)
            .WithMany(p => p.Languages)
            .HasForeignKey(pl => pl.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pl => pl.Language)
            .WithMany(l => l.Persons)
            .HasForeignKey(pl => pl.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pl => pl.ProficiencyLevel)
            .IsRequired()
            .HasDefaultValue(LanguageProficiency.Beginner)
            .HasConversion<int>();

        // Her kişinin sadece bir primary language'i olabilir
        builder.HasIndex(pl => new { pl.PersonId, pl.IsPrimary })
            .HasFilter("[IsPrimary] = 1 AND [IsDeleted] = 0")
            .IsUnique();
    }
} 