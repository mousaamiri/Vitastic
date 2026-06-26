using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.Areas.UserPanel.Controllers.Base
{
    [Area("UserPanel")]
    [Route("[controller]")]
    [Authorize]
    public abstract class UserController : Controller
    {
        protected Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
                return null;

            return Guid.TryParse(claim.Value, out Guid id) ? id : null;
        }

        protected IActionResult RedirectToMainLogin()
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        protected IActionResult? EnsureUserLoggedIn()
        {
            if (GetCurrentUserId() is null)
                return RedirectToMainLogin();

            return null;
        }

    }
}
