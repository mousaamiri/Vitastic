using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.UserPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.DTOs.UserProfile;

namespace Vitastic.Web.Areas.UserPanel.Controllers;

public class UserProfileController(IUserManagerService userManagerService, ILogger<UserProfileController> logger)
    : UserController
{
    #region Index

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId()!.Value;
        ApiResponse<UserDetailDto> userDetail = await userManagerService.GetUserAsync(userId);

        if (!userDetail.IsSuccess)
            return RedirectToMainLogin();

        return View(userDetail.Data);
    }

    #endregion

    #region EditProfile

    [HttpGet("edit")]
    public async Task<IActionResult> EditProfile()
    {
        var userId = GetCurrentUserId()!.Value;
        ApiResponse<UserDetailDto> userDetail = await userManagerService.GetUserAsync(userId);

        if (!userDetail.IsSuccess)
        {
            TempData["IsSuccess"] = false;
            TempData["Message"] = userDetail.Message;
        }

        return View(userDetail.Data);
    }

    [HttpPatch("edit-name")]
    public async Task<IActionResult> EditUserFullName([FromBody] UpdateProfileRequest dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, message = string.Join(" | ", errors) });
        }

        try
        {
            var userId = GetCurrentUserId()!.Value;
            ApiResponse userDetail = await userManagerService.EditUserProfile(userId, dto);

            if (!userDetail.IsSuccess)
            {
                return BadRequest(new { success = false, message = userDetail.Message ?? "خطا در بروزرسانی نام" });
            }

            return Ok(new { success = true, message = "تغییر نام و نام خانوادگی با موفقیت انجام شد" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user full name for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "خطا در بروزرسانی اطلاعات" });
        }
    }

    [HttpPatch("edit-phone-number")]
    public async Task<IActionResult> EditUserPhoneNumber([FromBody] UpdateProfileRequest dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, message = string.Join(" | ", errors) });
        }

        try
        {
            var userId = GetCurrentUserId()!.Value;
            ApiResponse userDetail = await userManagerService.EditUserProfile(userId, dto);

            if (!userDetail.IsSuccess)
            {
                return BadRequest(new { success = false, message = userDetail.Message ?? "خطا در بروزرسانی شماره تلفن" });
            }

            return Ok(new { success = true, message = "تغییر شماره تلفن با موفقیت انجام شد" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating phone number for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "خطا در بروزرسانی اطلاعات" });
        }
    }

    [HttpPatch("edit-email")]
    public async Task<IActionResult> EditUserEmail([FromBody] ChangeEmailRequest dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, message = string.Join(" | ", errors) });
        }

        try
        {
            var userId = GetCurrentUserId()!.Value;
            ApiResponse userDetail = await userManagerService.EditUserEmail(userId, dto);

            if (!userDetail.IsSuccess)
            {
                return BadRequest(new { success = false, message = userDetail.Message ?? "خطا در تغییر ایمیل" });
            }

            return Ok(new { success = true, message = "تغییر ایمیل کاربری با موفقیت انجام شد" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error changing email for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "خطا در بروزرسانی اطلاعات" });
        }
    }
    [HttpPatch("update-avatar")]
    public async Task<IActionResult> UpdateUserAvatarFile([FromForm] IFormFile avatarFile)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, message = string.Join(" | ", errors) });
        }

        try
        {
            
            var userId = GetCurrentUserId()!.Value;
            ApiResponse userDetail = await userManagerService.UpdateUserAvatar(userId, avatarFile);

            if (!userDetail.IsSuccess)
            {
                return BadRequest(new { success = false, message = userDetail.Message ?? "خطا در به‌روزرسانی آواتار" });
            }

            return Ok(new { success = true, message = "آواتار با موفقیت به‌روزرسانی شد" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating avatar for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "خطا در بروزرسانی اطلاعات" });
        }
    }
    #endregion

    #region ChangePassword
    [HttpGet("change-password")]
    public IActionResult ChangePassword() => View();

    [HttpPost("change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        [Bind("CurrentPassword,NewPassword,ConfirmPassword")] ChangePasswordDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return View(model);
        }

        if (!string.Equals(model.NewPassword, model.ConfirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "رمز عبور و تأیید رمز عبور مطابقت ندارند.");
            return View(model);
        }

        var result = await userManagerService.ChangePassword(model);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["IsCritical"] = true;
        TempData["Message"] = "رمز عبور با موفقیت تغییر یافت.";

        return View();
    }
    #endregion
}
