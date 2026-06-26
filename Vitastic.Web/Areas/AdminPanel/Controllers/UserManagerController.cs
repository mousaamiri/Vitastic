using Azure;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Areas.AdminPanel.Models.Users;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.DTOs.Role;
using Vitastic.Web.Models.DTOs.UserProfile;
using static Vitastic.Web.Areas.AdminPanel.Controllers.UserManagerController;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class UserManagerController(IUserManagerService userService,
    IRoleManagerService roleManagerService,
    ILogger<UserManagerController> logger) : AdminController
{
    #region Index

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string searchTerm = "", int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            PaginatedApiResponse<UserDto> response = await userService.SearchUsersAsync(searchTerm, pageNumber, pageSize);
            if (!response.IsSuccess)
            {

                ModelState.AddModelError(string.Empty, response.Message ?? "خطا در دریافت لیست کاربران.");

                var emptyPaginatedData = new PaginatedData<UserDto>();
                return View(emptyPaginatedData);
            }

            return View(response.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while loading the user list page.");
            ModelState.AddModelError(string.Empty, "خطای غیرمنتظره‌ای رخ داده است. لطفاً دوباره تلاش کنید.");

            var emptyPaginatedData = new PaginatedData<UserDto>();
            return View(emptyPaginatedData);
        }
    }

    #endregion

    #region Upsert User
    [HttpGet("{userId:guid}/upsert")]

    public async Task<IActionResult> UpsertUser(Guid userId, [FromQuery] Mode mode)
    {
        var model = new UserManagerViewModel
        {
            UserDetailDto = DefaultUser,
            Mode = mode
        };
        try
        {
            // ولیدیشن Mode
            if (!Enum.IsDefined(typeof(Mode), mode))
            {
                TempData["ErrorMessage"] = "مقدار Mode نامعتبر است.";
                return View("UpsertUser", model);
            }

            PaginatedApiResponse<RoleDto> roleResponse = await roleManagerService.GetRolesAsync(null);
            if (!roleResponse.IsSuccess)
            {
                logger.LogWarning("خطا در دریافت کاربر با ID: {UserId}", userId);
                TempData["ErrorMessage"] = "خطایی در بازیابی اطلاعات کاربر رخ داده است.";
                return View("UpsertUser", model);
            }
            model.Roles = roleResponse.Data!;
            // مدیریت حالت‌های Insert و Update
            if (mode == Mode.Update)
            {
                ApiResponse<UserDetailDto> response = await userService.GetUserAsync(userId);

                if (!response.IsSuccess)
                {
                    logger.LogWarning("خطا در دریافت کاربر با ID: {UserId}", userId);
                    TempData["ErrorMessage"] = "خطایی در بازیابی اطلاعات کاربر رخ داده است.";
                    return View("UpsertUser", model);
                }

                model.UserDetailDto = response.Data!;
                return View("UpsertUser", model);
            }
            else if (mode == Mode.Insert)
            {
                return View("UpsertUser", model);
            }

            return View("UpsertUser", model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "خطای سیستمی در UpsertUser");
            TempData["ErrorMessage"] = "خطای داخلی سرور رخ داده است.";
            return View("UpsertUser", model);
        }
    }
    [HttpPost("{userId:guid}/upsert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertUser(UserManagerViewModel dto, [FromForm] List<Guid> selectedRoles,
        [FromForm] IFormFile? avatarFile)
    {
        try
        {
            // Validate Mode
            if (!Enum.IsDefined(typeof(Mode), dto.Mode))
            {
                TempData["ErrorMessage"] = "مقدار Mode نامعتبر است.";
                return RedirectToAction("Index");
            }

            // Build request
            var request = new UpsertUserByAdminRequest(
                dto.UserDetailDto.UserName,
                dto.UserDetailDto.Email,
                dto.NewPassword,
                dto.UserDetailDto.FirstName,
                dto.UserDetailDto.LastName,
                dto.UserDetailDto.PhoneNumber,
                avatarFile,
                selectedRoles,
                dto.UserDetailDto.IsActive
            );

            ApiResponse response;

            if (dto.Mode == Mode.Update)
            {
                response = await userService.UpdateUserByAdmin(request, dto.UserDetailDto.Id);
            }
            else // Mode.Insert
            {
                ApiResponse<Guid> createResponse = await userService.CreateUserByAdmin(request);
                response = new ApiResponse
                {
                    IsSuccess = createResponse.IsSuccess,
                    Message = createResponse.Message
                };
            }

            // Handle success
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = dto.Mode == Mode.Insert
                    ? "کاربر با موفقیت ایجاد شد."
                    : "کاربر با موفقیت ویرایش شد.";

                return RedirectToAction("Index");
            }

            // Handle failure - reload roles for re-rendering view
            TempData["ErrorMessage"] = response.Message;

            PaginatedApiResponse<RoleDto> roleResponse = await roleManagerService.GetRolesAsync(null);
            dto.Roles = roleResponse.Data ?? new PaginatedData<RoleDto>();

            return View("UpsertUser", dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "خطای سیستمی در UpsertUser");
            TempData["ErrorMessage"] = "خطای داخلی سرور رخ داده است.";

            // Reload roles for error case
            PaginatedApiResponse<RoleDto> roleResponse = await roleManagerService.GetRolesAsync(null);
            dto.Roles = roleResponse.Data ?? new PaginatedData<RoleDto>();

            return View("UpsertUser", dto);
        }
    }




    #endregion

    #region Helper

    private static readonly UserDetailDto DefaultUser =
        new(Guid.Empty, "", "", null,
            null, null, 0, false,
            DateTimeOffset.UnixEpoch, null, [""], null);
    public enum Mode
    {
        Update,
        Insert,
        Delete,
        Read
    }

    #endregion
}
