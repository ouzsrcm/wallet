using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using walletv2.Data.DataContext;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Services;

namespace walletv2.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Walletv2DbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        await defaultUser(context, passwordHasher);

        await defaultExpenseTypes(context, passwordHasher);

        try
        {
            await context.SaveChangesAsync();
        }
        catch { }

    }


    /// <summary>
    /// add default user to the database if not exists.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="passwordHasher"></param>
    /// <returns></returns>
    private static async Task defaultUser(Walletv2DbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
            return;

        var (salt, hash) = passwordHasher.HashPassword("saricamou2");
        var adminUser = new User
        {
            Address = "123 Admin St, Admin City, Admin Country",
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
    }

    /// <summary>
    /// add default expense types to the database if not exists.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="passwordHasher"></param>
    /// <returns></returns>
    private static async Task defaultExpenseTypes(Walletv2DbContext context, IPasswordHasher passwordHasher)
    {
        if (context.CashflowTypes.Any())
            return;

        foreach (string item in WalletService.DefaultCashflowTypes)
            await context.IncomeExpenseTypes.AddAsync(new IncomeExpenseType
            {
                Name = item,
                Description = item,
            });
    }
}