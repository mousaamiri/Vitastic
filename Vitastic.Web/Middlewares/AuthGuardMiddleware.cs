namespace Vitastic.Web.Middlewares;

public class AuthGuardMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        // If API returns 401 and request is not AJAX
        if (context.Response.StatusCode == 401 &&
            !context.Request.Headers["X-Requested-With"].Contains("XMLHttpRequest"))
        {
            context.Response.Redirect(
                $"/Account/Login?returnUrl={Uri.EscapeDataString(context.Request.Path)}");
        }
        // Routs that need to auth
        if (path?.StartsWith("/user-panel") == true ||
            path?.StartsWith("/admin") == true)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.Redirect($"/Account/Login?returnUrl={context.Request.Path}");
                return;
            }

            // Role Management for Admin Panel
            //if (path.StartsWith("/admin") && !context.User.IsInRole("Admin"))
            //{
            //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //    context.Response.Redirect("/Error?code=403");
            //    return;
            //}
        }

        await next(context);
    }
}
