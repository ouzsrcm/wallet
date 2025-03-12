using Wallet.Services.DTOs.User;

namespace Wallet.Services.Abstract;

public interface IUserService
{
    // User CRUD
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> GetUserByUsernameAsync(string username);
    Task<UserDto> CreateUserAsync(CreateUserDto userDto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto userDto);
    Task DeleteUserAsync(Guid id);
    
    // Credential operations
    Task<UserCredentialDto> UpdateCredentialsAsync(Guid userId, UpdateUserCredentialDto credentialDto);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
    Task<bool> VerifyEmailAsync(Guid userId, string token);
    Task<bool> VerifyPhoneAsync(Guid userId, string token);
    Task<bool> ToggleTwoFactorAsync(Guid userId, bool enable, string? type = null);
    Task<bool> UpdateRolesAsync(Guid userId, IEnumerable<string> roles);
} 