namespace Wallet.Services.DTOs.Auth;

public class LoginResponseDto
{
    public AuthUserDto User { get; set; } = null!;
    public Guid PersonId { get; set; }
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
} 