using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Services;
using walletv2.Dtos;
using walletv2.Extensions;

namespace walletv2.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService _userService)
    {
        this._userService = _userService ?? throw new ArgumentNullException(nameof(_userService));
    }

    /// <summary>
    /// create a new user if not exists account with the provided details.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest param)
    {
        try
        {
            var res = await _userService.Register(new CreateUserDto
            {
                Email = param.Email ?? string.Empty,
                Username = param.Username ?? string.Empty,
                Password = param.Password
            });
            return Ok(new RegisterUserResponse()
            {
                Status = res == null ? ApiResponseStatus.Error : ApiResponseStatus.Success,
                UserId = res?.Id,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new RegisterUserResponse()
            {
                Status = ApiResponseStatus.Error,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// user profile details.
    /// </summary>
    /// <returns>UserDetailResponseDto</returns>
    [HttpGet("profile")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserDetailResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserDetailResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var details = _userService.GetUserDetails(User.GetUserId());
            return Ok(new UserDetailResponseDto()
            {
                Status = ApiResponseStatus.Success,
                User = await details
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new UserDetailResponseDto()
            {
                Status = ApiResponseStatus.Error,
                Message = ex.Message
            });
        }
    }
}