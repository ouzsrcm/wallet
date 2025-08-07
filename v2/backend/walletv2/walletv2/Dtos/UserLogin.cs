namespace walletv2.Dtos;

public class UserLoginRequest : BaseRequestDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class UserLoginResponse : BaseResponseDto
{
    public UserLoginResponse(string token, DateTime expirationDate)
    {
        this.Token = token;
        this.Expiration = expirationDate;
    }

    public required string Token { get; set; }
    public required DateTime Expiration { get; set; }
}