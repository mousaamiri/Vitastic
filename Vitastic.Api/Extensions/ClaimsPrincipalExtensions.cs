using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Vitastic.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    #region User Identity Methods

    /// <summary>
    /// Extracts the user ID from JWT claims. Throws if not authenticated.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.TryGetUserId();

        if (userId is null)
            throw new UnauthorizedAccessException("شناسه کاربر در توکن یافت نشد.");

        return userId.Value;
    }

    /// <summary>
    /// Tries to extract the user ID from JWT claims. Returns null if not found.
    /// </summary>
    public static Guid? TryGetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(claim))
            return null;

        return Guid.TryParse(claim, out var userId) ? userId : null;
    }

    #endregion

    #region Cart Identity Methods

    /// <summary>
    /// Returns the authenticated user's ID, or null for guest users.
    /// </summary>
    public static Guid? GetCurrentUserId(this ClaimsPrincipal user)
        => user.Identity?.IsAuthenticated == true ? user.TryGetUserId() : null;

    /// <summary>
    /// Extracts the session ID from context or request headers (for guest carts).
    /// </summary>
    public static string? GetSessionId(this HttpContext httpContext)
        => httpContext.Items["cart_session_id"] as string
           ?? httpContext.Request.Headers["X-Session-Id"].FirstOrDefault();


    #endregion
}
