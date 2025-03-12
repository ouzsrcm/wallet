using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class PersonContactConfiguration : IEntityTypeConfiguration<PersonContact>
{
    public void Configure(EntityTypeBuilder<PersonContact> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContactType)
               .HasMaxLength(20);

        builder.Property(x => x.ContactName)
               .HasMaxLength(100);

        builder.Property(x => x.ContactValue)
               .HasMaxLength(200);

        builder.Property(x => x.CountryCode)
               .HasMaxLength(5);

        builder.Property(x => x.AreaCode)
               .HasMaxLength(5);

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.Property(x => x.VerificationToken)
               .HasMaxLength(100);
    }
} 