using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class ReceiptItemConfiguration : IEntityTypeConfiguration<ReceiptItem>
{
    public void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Barcode)
               .HasMaxLength(50);

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(18,3)");

        builder.Property(x => x.Unit)
               .HasMaxLength(10);

        builder.Property(x => x.UnitPrice)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TotalPrice)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TaxRate)
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18,2)");
    }
} 