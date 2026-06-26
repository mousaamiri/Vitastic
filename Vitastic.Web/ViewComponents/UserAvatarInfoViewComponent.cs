using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Auth;

namespace Vitastic.Web.ViewComponents
{
    public class UserAvatarInfoViewComponent(IAuthService authService):ViewComponent
    {
        [ResponseCache(Duration = 3600 * 24, VaryByHeader = "Cookie")]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Claim? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return View(new UserAvatarInfoDto(
                    Guid.Empty,
                    "کاربر مهمان",
                    "",
                    "",
                    "/shared/panel/images/user.jpg",
                    ["بدون نقش"]
                ));
            }

            // request avatar info
            ApiResponse<UserAvatarInfoDto> response =
                await authService.GetAvatarInfoAsync(Guid.Parse(userId.Value));

            if (!response.IsSuccess)
            {
                return View(new UserAvatarInfoDto(
                    Guid.Empty,
                    "خطا در دریافت اطلاعات",
                    "خطا در دریافت اطلاعات",
                    "خطا در دریافت اطلاعات",
                    "/shared/panel/images/user.jpg",
                    [ "خطا در دریافت اطلاعات" ]
                ));
            }

            return View(response.Data);
       
    }
}
}
