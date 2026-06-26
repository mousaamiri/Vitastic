using System.Data;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Controllers;

public class AccountController(IAuthService authService, IMapper mapper) : Controller
{
    #region ==================== Private Helper ====================

    /// <summary>
    /// Adds API response errors to ModelState
    /// </summary>
    private void AddResponseErrorsToModelState(ApiResponse response)
    {
        foreach (var error in response.Errors)
            ModelState.AddModelError("", error);

        if (response.Errors.Count == 0)
            ModelState.AddModelError("", response.Message);
    }

    #endregion

    #region ==================== Register ====================

    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserModel req)
    {
        if (!ModelState.IsValid)
            return View(req);
        var activationLink = Url.Action("ActiveAccount","Account", null, Request.Scheme);
        RegisterDto dto = new RegisterDto(req.UserName,req.Email,req.Password, activationLink);
        ApiResponse response = await authService.RegisterAsync(dto);

        if (!response.IsSuccess)
        {
            AddResponseErrorsToModelState(response);
            return View(req);
        }

        ViewBag.IsSuccess = true;
        return View(req);
    }

    #endregion

    #region ==================== Login ====================

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        LoginDto dto = mapper.Map<LoginDto>(model);
        ApiResponse<LoginResponseDto> response = await authService.LoginAsync(dto);

        if (!response.IsSuccess || response.Data == null)
        {
            AddResponseErrorsToModelState(response);
            return View(model);
        }
        LoginResponseDto data = response.Data!;
      

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, data.UserId.ToString()),
            
            new("AccessToken", data.AccessToken),
            new("RefreshToken", data.RefreshToken),
            new("AccessTokenExpiresAt", data.AccessTokenExpiresAt.ToString(CultureInfo.InvariantCulture)),
            new("RefreshTokenExpiresAt", data.RefreshTokenExpiresAt.ToString(CultureInfo.InvariantCulture))
        };
        foreach (var role in data.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role)); // ← Critical for admin detection
        }
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = data.AccessTokenExpiresAt
            });

        return LocalRedirect(returnUrl ?? "/");
    }

    #endregion

    #region ==================== Active Account ====================

    [HttpGet("activate/{token}")]
    public async Task<IActionResult> ActiveAccount([FromRoute] string token, [FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            return NotFound();

        var dto = new ActivateUserDto(email, token);
        ApiResponse response = await authService.ActivateAsync(dto);

        ViewBag.IsActive = response.IsSuccess;

        if (!response.IsSuccess)
            AddResponseErrorsToModelState(response);

        return View();
    }

    #endregion

    #region ==================== Logout ====================

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Reset Password
    [HttpGet("ResetPassword")]
    public IActionResult ResetPassword([FromQuery] string token)
    {
        if (string.Equals(token, Guid.Empty.ToString(), StringComparison.Ordinal))
            return NotFound();

        return View(new ResetPasswordUserModel { Token = token });
    }

    [HttpPost("ResetPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordUserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new ResetPasswordDto(model.Token.ToString(), model.NewPassword);
        var response = await authService.ResetPasswordAsync(dto);
        ViewBag.IsSuccess = response.IsSuccess;
        if (!response.IsSuccess)
        {
            AddResponseErrorsToModelState(response);
            return View(model);
        }

        ViewBag.IsSuccess = true;
        return View(model);
    }
    #endregion

    #region ==================== Forgot Password ====================

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("ForgotPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgetPasswordUserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var dto = new ForgetPasswordDto(
            model.Email,
            Url.Action("ResetPassword", "Account", null, Request.Scheme)!
        );

        ApiResponse response = await authService.ForgetPasswordAsync(dto);

        if (!response.IsSuccess)
        {
            AddResponseErrorsToModelState(response);
            return View(model);
        }

        ViewBag.IsSuccess = true;
        return View(model);
    }

    #endregion

    #region ==================== Resend Activation Link ====================

    [HttpGet]
    public IActionResult ResendActivationLink()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendActivationLink(ResendActivationUserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var activationLink = Url.Action("ActiveAccount", "Account", null, Request.Scheme);
        var dto = new ResendActivationDto(model.Email, activationLink!);

        ApiResponse response = await authService.ResendActivationLinkAsync(dto);

        ViewBag.IsSuccess = response.IsSuccess;
        if (!response.IsSuccess)
        {
            AddResponseErrorsToModelState(response);
            return View(model);
        }

        TempData["SuccessMessage"] = "لینک فعالسازی مجدداً برای شما ارسال شد";
        return RedirectToAction("Login");
    }

    #endregion

}
