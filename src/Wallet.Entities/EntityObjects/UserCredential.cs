using Wallet.Entities.Base.Concrete;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wallet.Entities.EntityObjects;

public class UserCredential : SoftDeleteEntity
{
    // User Reference
    public Guid UserId { get; set; }
    public required User User { get; set; }
    
    // Authentication
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public bool RequirePasswordChange { get; set; }
    
    // Account Status
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsTwoFactorEnabled { get; set; }

    public string? TwoFactorType { get; set; } // Email, SMS, Authenticator
    public string? TwoFactorKey { get; set; }
    
    // Security
    public string? SecurityStamp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockoutEndDate { get; set; }
    public string? LastLoginIP { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? LastFailedLoginIP { get; set; }
    public DateTime? LastFailedLoginDate { get; set; }
    
    // Tokens
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpireDate { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpireDate { get; set; }
    public string? PhoneVerificationToken { get; set; }
    public DateTime? PhoneVerificationTokenExpireDate { get; set; }
    
    // Session
    public bool RememberMe { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public bool IsCurrentDevice { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public IEnumerable<string> Roles { get; set; } = new List<string>();
} 