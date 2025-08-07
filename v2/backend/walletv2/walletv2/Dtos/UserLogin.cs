namespace walletv2.Dtos;

public class UserLoginRequest : BaseRequestDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class UserLoginResponse : BaseResponseDto
{
    public UserLoginResponse(string accessToken, string refreshToken, DateTime expirationDate)
    {
        this.AccessToken = accessToken;
        this.RefreshToken = refreshToken;
        this.Expiration = expirationDate;
    }

    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime Expiration { get; set; }
}

public class RefreshTokenRequest : BaseRequestDto
{
    public required string RefreshToken { get; set; }
}