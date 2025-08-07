using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Models;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Repositories;
using walletv2.Data.Security;

namespace walletv2.Data.Services;

public interface IAuthService
{
    Task<UserLoginResponseDto> Login(UserLoginDto param);
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

        var tokens = JwtTokenGenerator.GenerateTokens(user.Id, user.Username ?? string.Empty, user.Email ?? string.Empty, _jwtSettings.Key, _jwtSettings.ExpireMinutes);

        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.RefreshToken))
            throw new InvalidOperationException("Failed to generate JWT tokens.");

        var exists = await _refreshTokenRepository.AnyAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsUsed);
        if (exists)
        {
            var existTokens = await _refreshTokenRepository.FindAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsUsed);
            foreach (var token in existTokens)
            {
                token.IsRevoked = true;
                token.IsUsed = true;
                await _refreshTokenRepository.UpdateAsync(token);
            }
        }

        var model = new RefreshToken
        {
            UserId = user.Id,
            Token = tokens.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
            IsRevoked = false,
            IsUsed = false
        };

        await _refreshTokenRepository.AddAsync(model);
        await _refreshTokenRepository.SaveChangesAsync();

        return await Task.FromResult(new UserLoginResponseDto()
        {
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
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
}
