using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.DataLayer.Configurations;

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
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptItem> ReceiptItems { get; set; }
    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ReceiptConfiguration());
        modelBuilder.ApplyConfiguration(new ReceiptItemConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
    }
} 