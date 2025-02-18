namespace Wallet.Services.DTOs.Auth;

public class ChangePasswordRequestDto
{
    public required string Email { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
} 