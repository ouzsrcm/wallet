using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Logging;
using Wallet.Services.Exceptions;

namespace Wallet.Api.Controllers;

/// <summary>
/// Authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService, 
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "username": "johndoe",
    ///         "password": "MySecureP@ss2024",
    ///         "rememberMe": true
    ///     }
    /// </remarks>
    /// <param name="request">Login credentials</param>
    /// <returns>User information and authentication tokens</returns>
    /// <response code="200">Returns the user data and tokens</response>
    /// <response code="400">If the credentials are invalid</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);
            
            var response = await _authService.LoginAsync(request);
            
            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return Ok(response);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Login failed for user {Username}: {Message}", 
                request.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for user {Username}", 
                request.Username);
            return StatusCode(500, new { message = "Giriş işlemi sırasında bir hata oluştu" });
        }
    }

    /// <summary>
    /// Token yenileme
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/auth/refresh-token
    ///     {
    ///         "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
    ///     }
    /// </remarks>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns>New JWT token and refresh token</returns>
    /// <response code="200">Returns new tokens</response>
    /// <response code="400">If the refresh token is invalid</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            _logger.LogInformation("Token refresh attempt");
            
            var response = await _authService.RefreshTokenAsync(refreshToken);
            
            _logger.LogInformation("Token refreshed successfully for user {UserId}", 
                response.User.Id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">If successfully logged out</response>
    /// <response code="401">If not authenticated</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            _logger.LogInformation("Logout attempt for user {UserId}", userId);
            
            await _authService.LogoutAsync(userId);
            
            _logger.LogInformation("User {UserId} logged out successfully", userId);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Şifre değiştirme
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/auth/change-password
    ///     {
    ///         "email": "john.doe@example.com",
    ///         "currentPassword": "OldP@ssw0rd",
    ///         "newPassword": "NewP@ssw0rd"
    ///     }
    /// </remarks>
    /// <param name="request">Password change request containing email and passwords</param>
    /// <returns>Success status</returns>
    /// <response code="200">If password was changed successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If not authenticated</response>
    /// <response code="404">If user or email not found</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            _logger.LogInformation("Password change attempt for user {UserId}", userId);
            
            var result = await _authService.ChangePasswordAsync(
                userId, 
                request.Email,
                request.CurrentPassword, 
                request.NewPassword
            );

            _logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return Ok(new { 
                success = true,
                message = "Password changed successfully",
                email = request.Email,
                timestamp = DateTime.UtcNow,
                requireRelogin = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", 
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                
            var statusCode = ex.Message switch
            {
                "User not found" => StatusCodes.Status404NotFound,
                "User credentials not found" => StatusCodes.Status404NotFound,
                "Email is not verified" => StatusCodes.Status400BadRequest,
                "Current password is incorrect" => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            return StatusCode(statusCode, new { 
                success = false,
                message = ex.Message,
                errorCode = ex switch
                {
                    ArgumentException => "INVALID_INPUT",
                    UnauthorizedAccessException => "UNAUTHORIZED",
                    _ => "INTERNAL_ERROR"
                },
                email = request.Email,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Kullanıcı kaydı
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/auth/register
    ///     {
    ///         "email": "john.doe@example.com",
    ///         "username": "johndoe",
    ///         "password": "MySecureP@ss2024"
    ///     }
    /// </remarks>
    /// <param name="request">Register request containing email, username, and password</param>
    /// <returns>Register response</returns>
    /// <response code="200">Returns register response</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If an error occurs during registration</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            _logger.LogInformation("Registration attempt for user: {Username}", request.Username);
            
            var response = await _authService.RegisterAsync(request);
            
            _logger.LogInformation("User {Username} registered successfully", request.Username);
            return Ok(response);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Registration failed for user {Username}: {Message}", 
                request.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for user {Username}", 
                request.Username);
            return StatusCode(500, new { message = "Kayıt işlemi sırasında bir hata oluştu" });
        }
    }
} 