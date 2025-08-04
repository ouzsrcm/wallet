using Microsoft.EntityFrameworkCore;
using walletv2.Data.Entities.Objects;

namespace walletv2.Data.DataContext;

public class Walletv2DbContext : DbContext
{

    public Walletv2DbContext(DbContextOptions<Walletv2DbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your entities here
        // Example: modelBuilder.Entity<User>().ToTable("Users");
        base.OnModelCreating(modelBuilder);
    }

    // Define DbSets for your entities
    public DbSet<User> Users { get; set; }

}
