using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities.Objects;

public class User : BaseEntityImplementation
{
    public User()
    {
        Email = string.Empty;
        Username = string.Empty;
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the specified user details.
    /// </summary>
    /// <remarks>This constructor initializes the user with essential details required for authentication and
    /// identification. Ensure that all parameters are valid and non-empty to avoid runtime errors.</remarks>
    /// <param name="id">The unique identifier for the user.</param>
    /// <param name="email">The email address of the user. Cannot be null or empty.</param>
    /// <param name="username">The username of the user. Cannot be null or empty.</param>
    /// <param name="passwordSalt">The cryptographic salt used for hashing the user's password. Cannot be null or empty.</param>
    /// <param name="passwordHash">The hashed representation of the user's password. Cannot be null or empty.</param>
    public User(Guid id, string email, string username, string passwordSalt, string passwordHash)
    {
        this.Id = id;
        Email = email;
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; }

    [Required]
    [MinLength(60)]
    [MaxLength(500)]
    public string PasswordHash { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(500)]
    public string PasswordSalt { get; set; }

    [MinLength(3)]
    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MinLength(3)]
    [MaxLength(50)]
    public string? LastName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [Url]
    public string? ProfilePictureUrl { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    public DateTime? LastLogin { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPhoneNumberVerified { get; set; } = false;
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }
}