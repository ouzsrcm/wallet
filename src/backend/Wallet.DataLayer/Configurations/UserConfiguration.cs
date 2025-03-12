using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        // One-to-One relationship with Person
        builder.HasOne(x => x.Person)
               .WithOne(x => x.User)
               .HasForeignKey<User>(x => x.PersonId)
               .OnDelete(DeleteBehavior.Cascade);

        // One-to-One relationship with UserCredential
        builder.HasOne(x => x.Credential)
               .WithOne(x => x.User)
               .HasForeignKey<UserCredential>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
} 