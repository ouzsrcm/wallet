using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.MiddleName)
               .HasMaxLength(50);

        builder.Property(x => x.Gender)
               .HasMaxLength(20);

        builder.Property(x => x.IdNumber)
               .HasMaxLength(20);

        builder.Property(x => x.TaxNumber)
               .HasMaxLength(20);

        builder.Property(x => x.ProfilePictureUrl)
               .HasMaxLength(500);

        builder.Property(x => x.Language)
               .HasMaxLength(10); // e.g., "en-US", "tr-TR"

        builder.Property(x => x.TimeZone)
               .HasMaxLength(50);

        builder.Property(x => x.Currency)
               .HasMaxLength(3); // ISO 4217 currency codes

        builder.Property(x => x.NationalityId)
               .HasColumnType("uniqueidentifier")
               .IsRequired(false);

        // Relationships
        builder.HasMany(p => p.Addresses)
               .WithOne(a => a.Person)
               .HasForeignKey(a => a.PersonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Contacts)
               .WithOne(c => c.Person)
               .HasForeignKey(c => c.PersonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Nationality)
               .WithMany()
               .HasForeignKey(x => x.NationalityId)
               .OnDelete(DeleteBehavior.SetNull);
    }
} 