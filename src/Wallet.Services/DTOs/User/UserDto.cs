namespace Wallet.Services.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool PushNotificationsEnabled { get; set; }
    public bool SMSNotificationsEnabled { get; set; }
    
    // Credential bilgileri
    public UserCredentialDto? Credential { get; set; }
}

public class UserCredentialDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorType { get; set; }
    public bool RequirePasswordChange { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class CreateUserDto
{
    public Guid PersonId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class UpdateUserDto
{
    public bool? EmailNotificationsEnabled { get; set; }
    public bool? PushNotificationsEnabled { get; set; }
    public bool? SMSNotificationsEnabled { get; set; }
}

public class UpdateUserCredentialDto
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsTwoFactorEnabled { get; set; }
    public string? TwoFactorType { get; set; }
    public IEnumerable<string>? Roles { get; set; }
} 