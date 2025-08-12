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
    private readonly ICurrencyService currencyService;

    public AuthController(IAuthService _authService, ICurrencyService currencyService)
    {
        this._authService = _authService ?? throw new ArgumentNullException(nameof(_authService));
        this.currencyService = currencyService;
    }

    /// <summary>
    /// login user with the provided credentials.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest param)
    {
        try
        {

            //await currencyService.UpdateDailyRates();

            var res = await _authService.Login(new UserLoginDto
            {
                Username = param.Username ?? string.Empty,
                Password = param.Password ?? string.Empty
            });
            return Ok(new UserLoginResponse(res.AccessToken, res.RefreshToken, res.Expiration)
            {
                Status = ApiResponseStatus.Success,
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken,
                Expiration = res.Expiration
            });
        }
        catch (Exception ex)
        {
            return BadRequest(await Task.FromResult(new UserLoginResponse(string.Empty, string.Empty, default)
            {
                Status = ApiResponseStatus.Error,
                Message = ex.Message,
                Expiration = DateTime.MinValue,
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            }));
        }
    }

    /// <summary>
    /// refresh JWT token using the provided refresh token.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest param)
    {
        try
        {
            var res = await _authService.GenerateTokenByRefreshToken(param.RefreshToken);
            return Ok(new UserLoginResponse(res.AccessToken, res.RefreshToken, res.Expiration)
            {
                Status = ApiResponseStatus.Success,
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken,
                Expiration = res.Expiration
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new UserLoginResponse(string.Empty, string.Empty, DateTime.MinValue)
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
