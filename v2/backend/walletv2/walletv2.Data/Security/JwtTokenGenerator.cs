using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace walletv2.Data.Security;

public class JwtTokenGenerator
{
    public record TokenResponse(string AccessToken, string RefreshToken);

    public static TokenResponse GenerateTokens(Guid userId, string username, string email, string secretKey, int expireMinutes = 60)
    {
        var accessToken = GenerateToken(userId, username, email, secretKey, expireMinutes);
        var refreshToken = GenerateRefreshToken();

        return new TokenResponse(accessToken, refreshToken);
    }

    public static string GenerateToken(Guid userId, string username, string email, string secretKey, int expireMinutes = 60)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "walletv2-api",
            audience: "walletv2-clients",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

}
