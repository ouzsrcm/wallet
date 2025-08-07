using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities.DataTransferObjects;

public class UserDto
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;

    [Required]
    public string? Username { get; set; } = string.Empty;

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }
}

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Password { get; set; } = string.Empty;
}

public class UserDetailedInfoDto
{
    public Guid Id { get; set; }
    public string? Fullname { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool isEmailVerified { get; set; }
    public bool isPhoneNumberVerified { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
}