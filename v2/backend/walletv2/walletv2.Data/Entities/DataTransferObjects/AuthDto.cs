using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities.DataTransferObjects;

public class UserLoginDto
{
    public string? Username { get; set; }
    [Required]
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class UserLoginResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime Expiration { get; set; }
    public UserDto? User { get; set; } = new();
}