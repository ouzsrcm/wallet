using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class PersonAddressConfiguration : IEntityTypeConfiguration<PersonAddress>
{
    public void Configure(EntityTypeBuilder<PersonAddress> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressType)
               .HasMaxLength(20);

        builder.Property(x => x.AddressName)
               .HasMaxLength(100);

        builder.Property(x => x.AddressLine1)
               .HasMaxLength(200);

        builder.Property(x => x.AddressLine2)
               .HasMaxLength(200);

        builder.Property(x => x.District)
               .HasMaxLength(100);

        builder.Property(x => x.City)
               .HasMaxLength(100);

        builder.Property(x => x.State)
               .HasMaxLength(100);

        builder.Property(x => x.Country)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.Property(x => x.Latitude)
               .HasPrecision(18, 15);

        builder.Property(x => x.Longitude)
               .HasPrecision(18, 15);
    }
} 