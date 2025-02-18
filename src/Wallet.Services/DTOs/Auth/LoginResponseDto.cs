namespace Wallet.Services.DTOs.Auth;

public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
} 