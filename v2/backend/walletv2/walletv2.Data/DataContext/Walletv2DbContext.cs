using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using walletv2.Data.Entities.Objects;

namespace walletv2.Data.DataContext;

public class Walletv2DbContext : DbContext
{
    public Walletv2DbContext(DbContextOptions<Walletv2DbContext> options) : base(options)
    {

    }

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
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<ExchangeRateType> ExchangeRateTypes { get; set; }

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