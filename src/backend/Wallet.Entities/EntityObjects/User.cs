using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class User : SoftDeleteEntity
{
    // Navigation Properties
    public Guid PersonId { get; set; }
    public Person? Person { get; set; }
    public UserCredential? Credential { get; set; }
    
    // Notifications (keeping these as they are user-specific preferences)
    public bool EmailNotificationsEnabled { get; set; }
    public bool PushNotificationsEnabled { get; set; }
    public bool SMSNotificationsEnabled { get; set; }
} 