namespace Vitastic.App.Common.Abstractions.Services.Base;

/// <summary>
/// Provides cart identity for both authenticated and guest users
/// </summary>
public interface ICartIdentityService
{
    /// <summary>
    /// Returns UserId for authenticated users, null for guests
    /// </summary>
    Guid? GetAuthenticatedUserId();

    /// <summary>
    /// Returns session ID for guest users (from cookie/header)
    /// </summary>
    string? GetGuestSessionId();

    /// <summary>
    /// Returns a unique cart identifier (UserId or SessionId)
    /// </summary>
    CartIdentity GetCartIdentity();
}
/// <summary>
/// Represents cart ownership - either by User or Guest Session
/// </summary>
public sealed class CartIdentity
{
    public Guid? UserId { get; init; }
    public string? SessionId { get; init; }
    public bool IsAuthenticated => UserId.HasValue;
    public bool IsGuest => !string.IsNullOrEmpty(SessionId) && !UserId.HasValue;

    public static CartIdentity ForUser(Guid userId)
        => new() { UserId = userId };

    public static CartIdentity ForGuest(string sessionId)
        => new() { SessionId = sessionId };

    public static CartIdentity Anonymous()
        => new();
}
