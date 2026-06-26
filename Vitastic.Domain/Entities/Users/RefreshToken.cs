using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Entities.Users;

public sealed class RefreshToken
{
    //Ignore this in database
    public static  int TokenMaxLength = 512;
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Token { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    // EF nav
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public static RefreshToken Create(
        UserId userId,
        string token,

        TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(lifetime)
        };
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
