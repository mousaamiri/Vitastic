using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Auth;

namespace Vitastic.Web.Areas.AdminPanel.Controllers
{
    public class DashboardController(IUserManagerService userManagerService) : AdminController
    {
        #region Index

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId()!.Value;
            ApiResponse<UserDetailDto> userDetail = await userManagerService.GetUserAsync(userId);

            return !userDetail.IsSuccess ? RedirectToMainLogin() : View(userDetail.Data);
        }

        #endregion

    }
}
