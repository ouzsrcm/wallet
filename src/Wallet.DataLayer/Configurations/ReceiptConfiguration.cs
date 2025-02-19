using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.StoreAddress)
               .HasMaxLength(500);

        builder.Property(x => x.TaxNumber)
               .HasMaxLength(20);

        builder.Property(x => x.ReceiptNo)
               .HasMaxLength(50);

        builder.Property(x => x.TotalAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PaymentDetails)
               .HasMaxLength(500);

        builder.Property(x => x.Notes)
               .HasMaxLength(1000);

        // Relationships
        builder.HasOne(x => x.Transaction)
               .WithOne()
               .HasForeignKey<Receipt>(x => x.TransactionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
               .WithOne(x => x.Receipt)
               .HasForeignKey(x => x.ReceiptId)
               .OnDelete(DeleteBehavior.Cascade);
    }
} 