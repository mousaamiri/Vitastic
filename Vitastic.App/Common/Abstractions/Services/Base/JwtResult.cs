namespace Vitastic.App.Common.Abstractions.Services.Base;

public sealed record JwtResult( Guid UserId,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    string[] Roles,
    DateTime RefreshTokenExpiresAt);
