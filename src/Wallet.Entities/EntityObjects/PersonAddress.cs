using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class PersonAddress : SoftDeleteEntity
{
    // Person Reference
    public Guid PersonId { get; set; }
    public Person? Person { get; set; }
    
    // Address Details
    public string? AddressType { get; set; } // Home, Work, Other
    public string? AddressName { get; set; } // Nickname for the address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public required string Country { get; set; }
    public required string PostalCode { get; set; }
    
    // Additional Info
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    
    // Coordinates
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
} 