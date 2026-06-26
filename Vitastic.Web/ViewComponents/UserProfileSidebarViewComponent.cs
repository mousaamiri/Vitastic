using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.ViewComponents
{
    public class UserProfileSidebarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var userName = UserClaimsPrincipal.Identity?.Name ?? "کاربر";
            var userEmail = UserClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        
            return View(new { UserName = userName, Email = userEmail });
        }
    }
}