namespace walletv2.Dtos;

public class RegisterUserRequest : BaseRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; } = false;
}

public class RegisterUserResponse : BaseResponseDto
{
    public Guid? UserId { get; set; }
}
