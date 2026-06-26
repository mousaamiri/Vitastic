using Vitastic.Domain.Entities.Users;

namespace Vitastic.App.Common.Abstractions.Services.Base;

public interface IJwTokenService
{
    JwtResult GenerateAccessToken(User user, string[] roles);
    string GenerateRefreshToken();
}
