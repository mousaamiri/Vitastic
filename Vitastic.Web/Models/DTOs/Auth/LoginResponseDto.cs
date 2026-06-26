namespace Vitastic.Web.Models.DTOs.Auth
{
    public sealed record LoginResponseDto(
        Guid UserId,
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        string[] Roles,
        DateTime RefreshTokenExpiresAt
        );
}
