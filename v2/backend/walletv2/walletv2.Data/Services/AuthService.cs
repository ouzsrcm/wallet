using Microsoft.Extensions.Options;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Models;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Repositories;
using walletv2.Data.Security;

namespace walletv2.Data.Services;

public interface IAuthService
{
    Task<UserLoginResponseDto> Login(UserLoginDto param);
    Task<UserLoginResponseDto> GenerateTokenByRefreshToken(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly JwtSettings _jwtSettings;

    private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;

    public AuthService(IUserService _userService, IOptions<JwtSettings> options, IBaseRepository<RefreshToken> _refreshTokenRepository)
    {
        this._refreshTokenRepository = _refreshTokenRepository;

        this._jwtSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this._userService = _userService ?? throw new ArgumentNullException(nameof(_userService));
    }

    /// <summary>
    /// login with the provided credentials and generate JWT tokens.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserLoginResponseDto> Login(UserLoginDto param)
    {
        var user = await _userService.ValidateUser(param);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password.");

        var (refreshToken, accessToken) = await GenerateToken(user);

        return await Task.FromResult(new UserLoginResponseDto()
        {
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        });
    }

    /// <summary>
    /// generates JWT tokens for the user and manages refresh tokens.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<(string, string)> GenerateToken(UserDto user)
    {
        var tokens = JwtTokenGenerator.GenerateTokens(user.Id, user.Username ?? string.Empty, user.Email ?? string.Empty, _jwtSettings.Key, _jwtSettings.ExpireMinutes);
        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.RefreshToken))
            throw new InvalidOperationException("Failed to generate JWT tokens.");

        if (await _refreshTokenRepository.AnyAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsUsed))
            foreach (var token in await _refreshTokenRepository.FindAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsUsed))
            {
                token.IsRevoked = true;
                token.IsUsed = true;
                await _refreshTokenRepository.UpdateAsync(token);
            }
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = tokens.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
            IsRevoked = false,
            IsUsed = false
        });
        await _refreshTokenRepository.SaveChangesAsync();

        return (tokens.RefreshToken, tokens.AccessToken);
    }

    /// <summary>
    /// generates a new access token using the provided refresh token or user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserLoginResponseDto> GenerateTokenByRefreshToken(string refreshToken)
    {
        var lastToken = (await _refreshTokenRepository.FindAsync(x => x.Token == refreshToken && !x.IsRevoked && !x.IsUsed)).FirstOrDefault();
        if (lastToken is null)
            throw new InvalidOperationException("Invalid or expired refresh token.");

        if (string.IsNullOrEmpty(lastToken.Token) || lastToken.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token not found or expired.");

        var user = await _userService.GetUserDetails(lastToken.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var userDto = new UserDto()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            Id = user.Id,
            LastName = user.LastName,
            Username = user.Username
        };
        var (_refreshToken, _accessToken) = await GenerateToken(userDto);
        return new UserLoginResponseDto()
        {
            AccessToken = _accessToken,
            RefreshToken = _refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            User = userDto
        };
    }
}
