using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class PersonContact : SoftDeleteEntity
{
    // Person Reference
    public Guid PersonId { get; set; }
    public Person? Person { get; set; }
    
    // Contact Details
    public string? ContactType { get; set; } // Phone, Email, Social Media, Other
    public string? ContactName { get; set; } // Nickname for the contact
    public string? ContactValue { get; set; } // Phone number, email address, social media handle
    
    // Phone Specific
    public string? CountryCode { get; set; }
    public string? AreaCode { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    
    // Email Specific
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    
    // Additional Info
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsPublic { get; set; }
    
    // Notifications
    public bool EmailNotificationsEnabled { get; set; }
    public bool SMSNotificationsEnabled { get; set; }
    
    // Verification
    public string? VerificationToken { get; set; }
    public DateTime? VerificationTokenExpireDate { get; set; }
} 