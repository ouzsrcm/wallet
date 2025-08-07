using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Services;
using walletv2.Dtos;

namespace walletv2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService _authService)
    {
        this._authService = _authService ?? throw new ArgumentNullException(nameof(_authService));
    }

    /// <summary>
    /// login user with the provided credentials.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status500InternalServerError)]
    public async Task<UserLoginResponse> Login([FromBody] UserLoginRequest param)
    {
        try
        {
            var res = await _authService.Login(new UserLoginDto
            {
                Username = param.Username ?? string.Empty,
                Password = param.Password ?? string.Empty
            });
            return new UserLoginResponse(res.AccessToken, res.RefreshToken, res.Expiration)
            {
                Status = ApiResponseStatus.Success,
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken,
                Expiration = res.Expiration
            };
        }
        catch (Exception ex)
        {
            return await Task.FromResult(new UserLoginResponse(string.Empty, string.Empty, default)
            {
                Status = ApiResponseStatus.Error,
                Message = ex.Message,
                Expiration = DateTime.MinValue,
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            });
        }
    }

}
