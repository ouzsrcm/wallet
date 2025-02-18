using Wallet.Services.DTOs.Auth;

namespace Wallet.Services.Abstract;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<bool> ValidateTokenAsync(string token);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, string email, string currentPassword, string newPassword);
} 