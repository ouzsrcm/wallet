using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.User;
using Microsoft.Extensions.Logging;
using Wallet.Services.Exceptions;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("User Management")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı bilgilerini ID'ye göre getirir
    /// </summary>
    /// <remarks>
    /// Örnek yanıt:
    /// 
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "emailNotificationsEnabled": true,
    ///         "pushNotificationsEnabled": true,
    ///         "smsNotificationsEnabled": true,
    ///         "credential": {
    ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "username": "johndoe",
    ///             "email": "john@example.com",
    ///             "phoneNumber": "+1234567890",
    ///             "isActive": true,
    ///             "isEmailVerified": true,
    ///             "isPhoneVerified": true,
    ///             "isTwoFactorEnabled": false,
    ///             "roles": ["User", "Admin"]
    ///         }
    ///     }
    /// </remarks>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting user details for {UserId}", id);
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                return NotFound(new { message = "User not found" });
            }
            
            _logger.LogInformation("Retrieved user details for {UserId}", id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user details for {UserId}", id);
            return StatusCode(500, new { message = "Kullanıcı bilgileri getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kullanıcı bilgilerini kullanıcı adına göre getirir
    /// </summary>
    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        try
        {
            _logger.LogInformation("Getting user details for username {Username}", username);
            
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User with username {Username} not found", username);
                return NotFound(new { message = "User not found" });
            }
            
            _logger.LogInformation("Retrieved user details for username {Username}", username);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user details for username {Username}", username);
            return StatusCode(500, new { message = "Kullanıcı bilgileri getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Yeni kullanıcı oluşturur
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/user
    ///     {
    ///         "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "username": "johndoe",
    ///         "password": "StrongP@ssw0rd",
    ///         "email": "john@example.com",
    ///         "phoneNumber": "+1234567890",
    ///         "roles": ["User"]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto userDto)
    {
        try
        {
            _logger.LogInformation("Creating new user with username {Username}", userDto.Username);
            
            var user = await _userService.CreateUserAsync(userDto);
            
            _logger.LogInformation("Created user {UserId} with username {Username}", 
                user.Id, userDto.Username);
                
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create user with username {Username}: {Message}", 
                userDto.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with username {Username}", userDto.Username);
            return StatusCode(500, new { message = "Kullanıcı oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/user/{id}
    ///     {
    ///         "emailNotificationsEnabled": true,
    ///         "pushNotificationsEnabled": true,
    ///         "smsNotificationsEnabled": false
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
    {
        try
        {
            _logger.LogInformation("Updating user {UserId}", id);
            
            var user = await _userService.UpdateUserAsync(id, userDto);
            
            _logger.LogInformation("Updated user {UserId}", id);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, new { message = "Kullanıcı güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kullanıcıyı siler (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kullanıcı kimlik bilgilerini günceller
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/user/{userId}/credentials
    ///     {
    ///         "email": "newemail@example.com",
    ///         "phoneNumber": "+9876543210",
    ///         "isActive": true,
    ///         "isTwoFactorEnabled": true,
    ///         "twoFactorType": "Email",
    ///         "roles": ["User", "Admin"]
    ///     }
    /// </remarks>
    [HttpPut("{userId}/credentials")]
    [ProducesResponseType(typeof(UserCredentialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserCredentialDto>> UpdateCredentials(
        Guid userId, 
        [FromBody] UpdateUserCredentialDto credentialDto)
    {
        try
        {
            _logger.LogInformation("Updating credentials for user {UserId}", userId);
            
            var credentials = await _userService.UpdateCredentialsAsync(userId, credentialDto);
            
            _logger.LogInformation("Updated credentials for user {UserId}", userId);
            return Ok(credentials);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found for credential update", userId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating credentials for user {UserId}", userId);
            return StatusCode(500, new { message = "Kullanıcı kimlik bilgileri güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kullanıcı şifresini değiştirir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/user/{userId}/change-password
    ///     {
    ///         "currentPassword": "OldP@ssw0rd",
    ///         "newPassword": "NewP@ssw0rd"
    ///     }
    /// </remarks>
    [HttpPost("{userId}/change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        Guid userId,
        [FromBody] ChangePasswordRequest request)
    {
        try
        {
            await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Email adresini doğrular
    /// </summary>
    [HttpPost("{userId}/verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail(Guid userId, [FromQuery] string token)
    {
        try
        {
            _logger.LogInformation("Email verification attempt for user {UserId}", userId);
            
            await _userService.VerifyEmailAsync(userId, token);
            
            _logger.LogInformation("Email verified successfully for user {UserId}", userId);
            return Ok(new { message = "Email verified successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for user {UserId}", userId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Telefon numarasını doğrular
    /// </summary>
    [HttpPost("{userId}/verify-phone")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPhone(Guid userId, [FromQuery] string token)
    {
        try
        {
            _logger.LogInformation("Phone verification attempt for user {UserId}", userId);
            
            await _userService.VerifyPhoneAsync(userId, token);
            
            _logger.LogInformation("Phone verified successfully for user {UserId}", userId);
            return Ok(new { message = "Phone number verified successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying phone for user {UserId}", userId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// İki faktörlü doğrulamayı açar/kapatır
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/user/{userId}/two-factor
    ///     {
    ///         "enable": true,
    ///         "type": "Email"  // Email, SMS, Authenticator
    ///     }
    /// </remarks>
    [HttpPost("{userId}/two-factor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleTwoFactor(
        Guid userId,
        [FromBody] TwoFactorRequest request)
    {
        try
        {
            await _userService.ToggleTwoFactorAsync(userId, request.Enable, request.Type);
            return Ok(new { message = $"Two-factor authentication {(request.Enable ? "enabled" : "disabled")} successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class TwoFactorRequest
{
    public bool Enable { get; set; }
    public string? Type { get; set; }
} 