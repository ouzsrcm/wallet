using Microsoft.Extensions.Options;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Models;
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

    public AuthService(IUserService _userService, IOptions<JwtSettings> options)
    {
        this._jwtSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this._userService = _userService ?? throw new ArgumentNullException(nameof(_userService));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserLoginResponseDto> Login(UserLoginDto param)
    {
        var user = await _userService.ValidateUser(param);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password.");

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Username ?? string.Empty, user.Email ?? string.Empty, _jwtSettings.Key, _jwtSettings.ExpireMinutes);
        var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);
        return await Task.FromResult(new UserLoginResponseDto()
        {
            Expiration = tokenExpiration,
            Token = token,
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
