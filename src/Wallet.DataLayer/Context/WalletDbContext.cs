using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;

namespace Wallet.DataLayer.Context;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<PersonAddress> PersonAddresses { get; set; }
    public DbSet<PersonContact> PersonContacts { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
} 