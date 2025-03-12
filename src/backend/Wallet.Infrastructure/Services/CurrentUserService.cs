using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wallet.Infrastructure.Abstract;

namespace Wallet.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId() =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetCurrentUserName() =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public string? GetIpAddress() =>
        _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    public string? GetUserAgent() =>
        _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

    public string? GetRequestUrl() =>
        _httpContextAccessor.HttpContext?.Request?.Path.ToString();

    public string? GetRequestMethod() =>
        _httpContextAccessor.HttpContext?.Request?.Method;
} 