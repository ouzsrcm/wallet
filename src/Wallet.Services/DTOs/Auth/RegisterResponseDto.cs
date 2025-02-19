namespace Wallet.Services.DTOs.Auth;

public class RegisterResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public AuthUserDto? User { get; set; }
} 