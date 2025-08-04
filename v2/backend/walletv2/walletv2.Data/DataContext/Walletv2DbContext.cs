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
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }

}
