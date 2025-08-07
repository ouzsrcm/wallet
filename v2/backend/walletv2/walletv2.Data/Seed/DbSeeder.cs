using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using walletv2.Data.DataContext;
using walletv2.Data.Entities.Objects;

namespace walletv2.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Walletv2DbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var (salt, hash) = passwordHasher.HashPassword("saricamou2");

            var adminUser = new User
            {
                Address= "123 Admin St, Admin City, Admin Country",
                Email = "os@mail.com",
                Bio = "Admin user for the application",
                FirstName = "Admin",
                City = "Admin City",
                LastName = "User",
                PhoneNumber = "+1234567890",
                ProfilePictureUrl = "https://example.com/profile.jpg",
                Username = "admin",
                PasswordSalt = salt,
                PasswordHash = hash,
                IsEmailVerified = true,
                IsPhoneNumberVerified = true,
                DateOfBirth = new DateTime(1990, 1, 1).ToUniversalTime(),
                LastLogin = DateTime.UtcNow,
            };

            await context.Users.AddAsync(adminUser);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception x)
            {
                throw;
            }
        }
    }
}