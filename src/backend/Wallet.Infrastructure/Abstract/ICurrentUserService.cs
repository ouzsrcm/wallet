namespace Wallet.Infrastructure.Abstract;

public interface ICurrentUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserName();
    string? GetIpAddress();
    string? GetUserAgent();
    string? GetRequestUrl();
    string? GetRequestMethod();
} 