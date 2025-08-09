using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using walletv2.Data.Entities.Objects;

namespace walletv2.Data.DataContext;

public class Walletv2DbContext : DbContext
{
    public Walletv2DbContext(DbContextOptions<Walletv2DbContext> options) : base(options)
    { }

    //TODO: bu kısım farkılı bir yere taşınacak.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasIndex(u =>
            new
            {
                u.Email,
                u.Username
            })
            .IsUnique();

        modelBuilder.Entity<Currency>()
            .HasIndex(c => c.CurrencyCode)
            .IsUnique();

        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(er => new { er.CurrencyId, er.ExchangeRateTypeId, er.RateDate })
            .IsUnique();

        modelBuilder.Entity<Cashflow>()
            .HasIndex(cf => new
            {
                cf.UserId,
                cf.CashflowTypeId
            })
            .IsUnique();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<ExchangeRateType> ExchangeRateTypes { get; set; }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Cashflow> Cashflows { get; set; }
    public DbSet<CashflowType> CashflowTypes { get; set; }
    public DbSet<IncomeExpense> IncomeExpenses { get; set; }
    public DbSet<IncomeExpenseType> IncomeExpenseTypes { get; set; }
    public DbSet<CashflowDocument> CashflowDocuments { get; set; }


    public DbSet<RefreshToken> RefreshTokens { get; set; }
}

public class Walletv2DbContextFactory : IDesignTimeDbContextFactory<Walletv2DbContext>
{
    public Walletv2DbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<Walletv2DbContext>();

        var connectionString = configuration.GetConnectionString("Walletv2DbContext");
        optionsBuilder.UseNpgsql(connectionString);

        return new Walletv2DbContext(optionsBuilder.Options);
    }
}