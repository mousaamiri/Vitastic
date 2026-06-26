using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Vitastic.Web.Infrastructure.Services;

namespace Vitastic.Web.Middlewares;

public class TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var accessTokenExpiry = context.User.FindFirst("AccessTokenExpiresAt")?.Value;

            if (!string.IsNullOrEmpty(accessTokenExpiry) &&
                DateTime.Parse(accessTokenExpiry) <= DateTime.UtcNow.AddMinutes(5))
            {
                var refreshToken = context.User.FindFirst("RefreshToken")?.Value;

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        var response = await authService.RefreshTokenAsync(refreshToken);

                        if (response is { IsSuccess: true, Data: not null })
                        {
                            var data = response.Data;
                            var claims = new List<Claim>
                            {
                                new(ClaimTypes.NameIdentifier, context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                                new("AccessToken", data.AccessToken),
                                new("RefreshToken", data.RefreshToken),
                                new("AccessTokenExpiresAt", data.AccessTokenExpiresAt.ToString(CultureInfo.InvariantCulture)),
                                new("RefreshTokenExpiresAt", data.RefreshTokenExpiresAt.ToString(CultureInfo.InvariantCulture))
                            };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            await context.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                principal,
                                new AuthenticationProperties
                                {
                                    IsPersistent = true,
                                    ExpiresUtc = data.AccessTokenExpiresAt
                                });
                        }
                        else
                        {
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "خطا در refresh token");
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
            }
        }

        await next(context);
    }
}