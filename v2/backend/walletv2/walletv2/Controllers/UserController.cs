using Microsoft.AspNetCore.Mvc;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Services;
using walletv2.Dtos;

namespace walletv2.Controllers;

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
    public async Task<RegisterUserResponse> RegisterUser([FromBody] RegisterUserRequest param)
    {
        try
        {
            var res = await _userService.Register(new CreateUserDto
            {
                Email = param.Email ?? string.Empty,
                Username = param.Username ?? string.Empty,
                Password = param.Password
            });
            return new RegisterUserResponse()
            {
                Status = res == null ? ApiResponseStatus.Error : ApiResponseStatus.Success,
                UserId = res?.Id,
            };
        }
        catch (Exception ex)
        {
            return new RegisterUserResponse()
            {
                Status = ApiResponseStatus.Error,
                Message = ex.Message
            };
        }
    }
}