using Microsoft.EntityFrameworkCore;
using walletv2.Data.DataContext;
using walletv2.Data.Entities.Models;
using walletv2.Data.Repositories;
using walletv2.Data.Seed;
using walletv2.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.AddDbContext<Walletv2DbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Walletv2DbContext")
        )
);

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<,>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await DbSeeder.SeedAsync(app.Services);

app.Run();

