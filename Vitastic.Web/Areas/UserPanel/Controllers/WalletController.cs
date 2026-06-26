
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.UserPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Wallet;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Areas.UserPanel.Controllers;

public class WalletController(IWalletService walletService) : UserController
{

    #region Index

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            return RedirectToMainLogin();

        try
        {
            var userId = Guid.Parse(userIdString);

            var result = await walletService.InitWalletAsync(userId, page, pageSize);
            if (!result.IsSuccess)
                TempData["ErrorMessage"] = result.Errors.FirstOrDefault() is null ? result.Errors.First(): result.Message;
            return View(result.Data);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"خطا در دریافت اطلاعات کیف پول: {ex.Message}");
            return View(new WalletInfoModel());
        }
    }

    #endregion


    #region Charge

    [HttpPost("{walletId:guid}/charge")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Charge([FromRoute]Guid walletId,[FromForm] AddFundsRequest viewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "اطلاعات وارد شده نامعتبر است.";
            return RedirectToAction(nameof(Index));
        }

        if (viewModel.Amount <= 0)
        {
            TempData["ErrorMessage"] = "مبلغ شارژ باید بیشتر از صفر باشد.";
            return RedirectToAction(nameof(Index));
        }
        try
        {

            // Build absolute callback URL for payment gateway
            var callbackUrl = Url.Action(action: nameof(OnlinePayment), controller: "Wallet", values: null,
                protocol: Request.Scheme, host: Request.Host.ToString());

            var paymentUrl =
                await walletService.ChargeWalletAsync(walletId,viewModel with{CallbackUrl = callbackUrl });
            if (!paymentUrl.IsSuccess)
            {
                TempData["ErrorMessage"] = "در دریافت آدرس پرداخت مشکلی پیش آمد.";
                return RedirectToAction(nameof(Index));
            }

            return Redirect(paymentUrl.Data);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"خطا در پردازش پرداخت: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Online payment

    [HttpGet("online-payment")]
    [AllowAnonymous]
    public async Task<IActionResult> OnlinePayment(
        [FromQuery] string authority,
        [FromQuery] string status,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(authority) || string.IsNullOrEmpty(status))
        {
            TempData["Error"] = "پارامترهای بازگشت از درگاه نامعتبر هستند.";
            TempData["RetryButtonText"] = "بازگشت به کیف پول";
            TempData["RetryButtonUrl"] = Url.Action("Index", "Wallet");
            return RedirectToAction("Result", "Payment", new { success = false });
        }

        var result = await walletService.VerifyWalletChargeAsync(authority, status, null, ct);

        if (result.IsSuccess)
        {
            TempData["Success"] = "شارژ کیف پول با موفقیت انجام شد، حدود 30 ثانیه دیگه به کیف پول شما مینشیند.";
            TempData["PrimaryButtonText"] = "بازگشت به کیف پول";
            TempData["PrimaryButtonUrl"] = Url.Action("Index", "Wallet");

            TempData["SecondaryButtonText"] = "صفحه اصلی";
            TempData["SecondaryButtonUrl"] = Url.Action("Index", "Home");
            return RedirectToAction("Result", "Payment", new { success = true });
        }

        TempData["Error"] = result.Message ?? "پرداخت ناموفق بود یا لغو شد.";
        TempData["RetryButtonText"] = "بازگشت به کیف پول";

        TempData["RetryButtonUrl"] = Url.Action("Index", "Wallet");
        return RedirectToAction("Result", "Payment", new { success = false });
    }

    #endregion



}

