using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.Property(x => x.Icon)
                   .HasMaxLength(50);

            builder.Property(x => x.Color)
                   .IsRequired()
                   .HasMaxLength(7); // #RRGGBB format

            // Self-referencing relationship for hierarchical categories
            builder.HasOne(x => x.ParentCategory)
                   .WithMany(x => x.SubCategories)
                   .HasForeignKey(x => x.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 