using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class Person : SoftDeleteEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required string Gender { get; set; }
    public Guid? NationalityId { get; set; }
    public string? TaxNumber { get; set; }
    public string? IdNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    
    // Preferences
    public string? Language { get; set; }
    public string? TimeZone { get; set; }
    public string? Currency { get; set; }
    
    // Navigation Properties
    public ICollection<PersonAddress>? Addresses { get; set; }
    public ICollection<PersonContact>? Contacts { get; set; }
    public User? User { get; set; }  // One-to-One relationship with User
    public Nationality? Nationality { get; set; }
} 