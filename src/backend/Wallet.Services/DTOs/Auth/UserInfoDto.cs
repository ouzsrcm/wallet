namespace Wallet.Services.DTOs.Auth;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
} 