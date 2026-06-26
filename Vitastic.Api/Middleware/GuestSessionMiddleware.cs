namespace Vitastic.Api.Middleware;

public class GuestSessionMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Cart-Session-Id";
    private const string ItemKey = "cart_session_id";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var sessionId)
                && !string.IsNullOrEmpty(sessionId))
            {
                context.Items[ItemKey] = sessionId.ToString();
            }

            await next(context);
        }
        catch (OperationCanceledException) { }
    }
}
