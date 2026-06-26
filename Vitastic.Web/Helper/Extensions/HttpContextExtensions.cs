namespace Vitastic.Web.Helper.Extensions;

public static class HttpContextExtensions
{
    public static string? GetAccessToken(this HttpContext context)
        => context.User.FindFirst("AccessToken")?.Value;

    public static string? GetRefreshToken(this HttpContext context)
        => context.User.FindFirst("RefreshToken")?.Value;
}
