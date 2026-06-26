// Infrastructure/Web/Services/CartIdentityService.cs

using Vitastic.Api.Extensions;
using Vitastic.App.Common.Abstractions.Services.Base;

namespace Vitastic.Api.Services;

internal sealed class CartIdentityService(IHttpContextAccessor httpContextAccessor) : ICartIdentityService
{
    public Guid? GetAuthenticatedUserId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        return httpContext.User.TryGetUserId();
    }

    public string? GetGuestSessionId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        return httpContext.GetSessionId();
    }

    public CartIdentity GetCartIdentity()
    {
        var userId = GetAuthenticatedUserId();
        if (userId.HasValue)
            return CartIdentity.ForUser(userId.Value);

        var sessionId = GetGuestSessionId();
        if (!string.IsNullOrEmpty(sessionId))
            return CartIdentity.ForGuest(sessionId);

        return CartIdentity.Anonymous();
    }
}
