using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;

namespace Vitastic.Web.ViewComponents
{
    public class UserAvatarViewComponent(IAuthService authApi) : ViewComponent
    {
        [ResponseCache(Duration = 3600 * 24, VaryByHeader = "Cookie")]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.IsFail = false;
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return View("Default","~/shared/panel/images/user.jpg");
            var userId = Guid.Parse(userIdString);
           
                ApiResponse<string> avatar = await authApi.GetUserAvatarAsync(userId);
                if(!avatar.IsSuccess)
                    return View("Default","~/shared/panel/images/user.jpg");
                return View("Default",avatar.Data);
            
        }
    }
}
