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

//TODO: user detailed DTOs if required