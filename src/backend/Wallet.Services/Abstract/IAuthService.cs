using Wallet.Services.DTOs.Auth;

namespace Wallet.Services.Abstract;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<bool> ValidateTokenAsync(string token);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, string email, string currentPassword, string newPassword);
    Task<UserInfoDto> GetUserInfoAsync(Guid userId);
    public (string hash, string salt) CreatePasswordHash(string password);
    public (bool, string) VerifyPassword(string password, string passwordHash);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
} 