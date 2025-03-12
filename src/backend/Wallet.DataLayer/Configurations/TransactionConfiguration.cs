using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
       public void Configure(EntityTypeBuilder<Transaction> builder)
       {
       builder.HasKey(x => x.Id);

       builder.Property(x => x.Amount)
              .HasColumnType("decimal(18,2)");

       builder.Property(x => x.Currency)
              .IsRequired()
              .HasMaxLength(3);

       builder.Property(x => x.Description)
              .IsRequired()
              .HasMaxLength(500);

       builder.Property(x => x.Reference)
              .HasMaxLength(100);

       // Relationships
       builder.HasOne(x => x.Person)
              .WithMany()
              .HasForeignKey(x => x.PersonId)
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(x => x.Category)
              .WithMany(x => x.Transactions)
              .HasForeignKey(x => x.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);
       }
}