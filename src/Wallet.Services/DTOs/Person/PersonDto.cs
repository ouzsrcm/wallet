namespace Wallet.Services.DTOs.Person;

public class PersonDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? NationalityId { get; set; }
    public string? TaxNumber { get; set; }
    public string? IdNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Language { get; set; }
    public string? TimeZone { get; set; }
    public string? Currency { get; set; }
    
    public List<PersonAddressDto>? Addresses { get; set; }
    public List<PersonContactDto>? Contacts { get; set; }
}

public class PersonAddressDto
{
    public Guid Id { get; set; }
    public string? AddressType { get; set; }
    public string? AddressName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public bool IsDefault { get; set; }
}

public class PersonContactDto
{
    public Guid Id { get; set; }
    public string? ContactType { get; set; }
    public string? ContactName { get; set; }
    public string? ContactValue { get; set; }
    public string? CountryCode { get; set; }
    public string? AreaCode { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPrimary { get; set; }
} 