namespace Wallet.Services.DTOs.Auth;

public class LoginRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
} 