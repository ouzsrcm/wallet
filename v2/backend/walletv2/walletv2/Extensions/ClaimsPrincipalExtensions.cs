using System.Security.Claims;

namespace walletv2.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;

        throw new InvalidOperationException("User ID claim is missing or invalid.");
    }
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.Name)?.Value;
    }
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.Email)?.Value;
    }
}
