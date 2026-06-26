
namespace Vitastic.Web.Middlewares;

public class GuestSessionMiddleware(RequestDelegate next)
{
    private const string Key = "cart_session_id";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // If not authenticated and no session ID exists, create one
            if (context.User.Identity?.IsAuthenticated != true)
            {
                var sessionId = context.Session.GetString(Key) ?? context.Request.Cookies[Key];

                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString("N");

                    context.Response.Cookies.Append(Key, sessionId, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax, // Strict مشکل cross-tab داره
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });
                }

                // Always sync to session for same-request access
                context.Session.SetString(Key, sessionId);
            }

            await next(context);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
