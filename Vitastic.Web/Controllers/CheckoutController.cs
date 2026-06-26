using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Checkout;
using Vitastic.Web.Models.DTOs.Transaction;

namespace Vitastic.Web.Controllers;

[Authorize]
[Route("[controller]")]
public class CheckoutController(
    ICheckoutService checkoutService,
    ILogger<CheckoutController> logger,
    ITransactionService transactionService) : Controller
{
    #region Index (GET) — Load checkout page

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return RedirectToAction("Login", "Account");

        logger.LogInformation("در حال بارگذاری صفحه پرداخت برای کاربر {UserId}", userId);

        var model = await checkoutService.GetCheckoutDataAsync(userId, ct);

        // Check if cart is empty
        if (!model.IsSuccess || model.Data!.Order.Items.Count == 0)
        {
            TempData["Warning"] = "سبد خرید شما خالی است";
            return RedirectToAction("Index", "Cart");
        }

        return View(model);
    }

    #endregion

    #region Submit (POST) — Process checkout and redirect to gateway

    [HttpPost("submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        [FromForm] CheckoutSubmitDto dto,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return RedirectToAction("Login", "Account");

        // Validation check
        if (!ModelState.IsValid)
        {
            logger.LogWarning("فرم پرداخت نامعتبر — کاربر {UserId}", userId);

            var model = await checkoutService.GetCheckoutDataAsync(userId, ct);
            if (model is null)
                return RedirectToAction("Index", "Cart");

            return View("Index", model);
        }

        logger.LogInformation(
            "پردازش پرداخت سفارش {OrderId} برای کاربر {UserId}",
            dto.OrderId, userId);

        // Build the callback URL (where user returns AFTER payment gateway)
        var callbackUrl = Url.Action(
            action: "Verify",
            controller: "Checkout",
            values: null,
            protocol: Request.Scheme)!;

        // Assign callback to DTO so service can send it to payment API
        dto.CallbackUrl = callbackUrl;

        // Execute the full checkout flow
        var result =
            await checkoutService.SubmitPaymentAsync(dto, userId, ct);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message ?? "خطا در پردازش پرداخت";
            return RedirectToAction("Index");
        }

        // Redirect user to payment gateway (paymentUrl)
        return Redirect(result.Data!);
    }

    #endregion

    #region Verify (GET) — Callback from payment gateway

    [AllowAnonymous]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify(
        [FromQuery] string authority,
        [FromQuery] string status,
        CancellationToken ct)
    {
        logger.LogInformation(
            "بازگشت از درگاه — Authority: {Authority}, Status: {Status}",
            authority, status);

        var request = new VerifyAndCompletePaymentRequest(authority, status);
        var result = await transactionService.VerifyPaymentAsync(request, ct);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message ?? "خطا در بررسی وضعیت پرداخت";
            // Retry button → back to checkout
            TempData["RetryButtonText"] = "تلاش مجدد";
            TempData["RetryButtonUrl"] = Url.Action("Index", "Checkout");
            // Secondary button → back to cart
            TempData["SecondaryButtonText"] = "بازگشت به سبد خرید";
            TempData["SecondaryButtonUrl"] = Url.Action("Index", "Cart");
            return RedirectToAction("Result", "Payment", new { success = false });
        }

        if (!result.Data!.IsSuccess)
        {
            TempData["Error"] = "پرداخت تأیید نشد. در صورت کسر مبلغ، ظرف ۷۲ ساعت به حساب شما بازمی‌گردد.";
            TempData["RetryButtonText"] = "تلاش مجدد";
            TempData["RetryButtonUrl"] = Url.Action("Index", "Checkout");

            TempData["SecondaryButtonText"] = "بازگشت به سبد خرید";
            TempData["SecondaryButtonUrl"] = Url.Action("Index", "Cart");
            return RedirectToAction("Result", "Payment", new { success = false });
        }

        // Order completed — show success
        TempData["Success"] = "پرداخت با موفقیت انجام شد! دسترسی شما فعال شد.";
        TempData["PrimaryButtonText"] = "مشاهده دوره‌های من";
        TempData["PrimaryButtonUrl"] = Url.Action("Index", "Course");

        TempData["SecondaryButtonText"] = "صفحه اصلی";
        TempData["SecondaryButtonUrl"] = Url.Action("Index", "Home");
        return RedirectToAction("Result", "Payment", new { success = true });
    }

    #endregion


    #region Private Helpers

    /// <summary>
    /// Extract current user ID from JWT/Cookie claims
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    #endregion
}
