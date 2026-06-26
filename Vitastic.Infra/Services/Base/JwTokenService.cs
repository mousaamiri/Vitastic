using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Users;

namespace Vitastic.Infra.Services.Base;

internal sealed class JwTokenService(IOptions<JwtSettings> options)
    : IJwTokenService
{
    private readonly JwtSettings _settings = options.Value;

    public JwtResult GenerateAccessToken(User user, string[] roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Name, user.UserName.Value),
            new(JwtRegisteredClaimNames.Name, user.UserName.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role)); // ← Critical for admin detection
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime accessExpires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var refreshToken = GenerateRefreshToken();
        DateTime refreshExpires = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessExpires,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = credentials,
        };

        var handler = new JsonWebTokenHandler();
        var accessToken = handler.CreateToken(descriptor);

        return new JwtResult(
            UserId: user.Id,
            AccessToken: accessToken,
            AccessTokenExpiresAt: accessExpires,
            RefreshToken: refreshToken,
            Roles:roles,
            RefreshTokenExpiresAt: refreshExpires);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
