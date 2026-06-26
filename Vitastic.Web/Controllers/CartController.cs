using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Cart;
using Vitastic.Web.Models.DTOs.Disocunt;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Controllers;


[Route("[controller]")]
public class CartController(
    ICartService cartService,
    IDiscountService discountService,
    ILogger<CartController> logger) : Controller
{
    #region Constants
    private const decimal TaxRate = 0.09m;
    private const decimal VatRate = 0.09m;
    private const decimal FreeShippingThreshold = 1_000_000m;
    private const decimal StandardShippingCost = 50_000m;
    #endregion

    #region Index - Display Cart Page

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? coupon)
    {
        try
        {
            ApiResponse<CartDto> cartResponse = await cartService.GetCart();

            if (!cartResponse.IsSuccess || cartResponse.Data is null)
            {
                TempData["Error"] = cartResponse.Errors?.FirstOrDefault()
                                    ?? "خطا در دریافت سبد خرید.";
                return View(new CartViewModel());
            }

            CartDto cart = cartResponse.Data;
            CartSummaryDto summary = BuildSummary(cart, coupon, discountResult: null);

            // Apply coupon if provided via query string
            if (!string.IsNullOrWhiteSpace(coupon))
            {
                ApiResponse<DiscountCalculationDto> discountResponse = await discountService.CalculateDiscountAsync(
                    coupon, cart.ItemsTotal, CancellationToken.None);

                if (discountResponse is { IsSuccess: true, Data: not null })
                {
                    summary = BuildSummary(cart, coupon, discountResponse.Data);
                }
                else
                {
                    TempData["Warning"] = discountResponse.Message ?? "کد تخفیف نامعتبر است";
                }
            }

            var viewModel = new CartViewModel
            {
                Cart = cart,
                Summary = summary
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت سبد خرید");
            TempData["Error"] = "خطا در دریافت سبد خرید";
            return View(new CartViewModel());
        }
    }

    #endregion

    #region Apply Coupon (AJAX)

    [HttpPost("apply-coupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CouponCode))
                return BadRequest(ApiResponse<CartSummaryDto>.Fail("لطفاً کد تخفیف را وارد کنید"));

            // Get current cart to have fresh subtotal
            ApiResponse<CartDto> cartResponse = await cartService.GetCart();
            if (!cartResponse.IsSuccess || cartResponse.Data is null)
                return BadRequest(ApiResponse<CartSummaryDto>.Fail("خطا در دریافت سبد خرید"));

            CartDto cart = cartResponse.Data;

            // Call discount API
            ApiResponse<DiscountCalculationDto> discountResponse = await discountService.CalculateDiscountAsync(
                request.CouponCode, cart.ItemsTotal, CancellationToken.None);

            if (!discountResponse.IsSuccess || discountResponse.Data is null)
                return BadRequest(ApiResponse<CartSummaryDto>.Fail(
                    discountResponse.Message ?? "کد تخفیف نامعتبر است",
                    errors: discountResponse.Errors));

            // Build full summary with discount applied
            CartSummaryDto summary = BuildSummary(cart, request.CouponCode, discountResponse.Data);

            return Ok(ApiResponse<CartSummaryDto>.Success(summary, "کد تخفیف با موفقیت اعمال شد"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال کد تخفیف {Code}", request.CouponCode);
            return StatusCode(500, ApiResponse<CartSummaryDto>.Fail("خطا در پردازش کد تخفیف"));
        }
    }

    #endregion

    #region Remove Coupon (AJAX)

    [HttpDelete("remove-coupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        try
        {
            ApiResponse<CartDto> cartResponse = await cartService.GetCart();
            if (!cartResponse.IsSuccess || cartResponse.Data is null)
                return BadRequest(ApiResponse<CartSummaryDto>.Fail("خطا در دریافت سبد خرید"));

            // Build summary without any discount
            CartSummaryDto summary = BuildSummary(cartResponse.Data, couponCode: null, discountResult: null);

            return Ok(ApiResponse<CartSummaryDto>.Success(summary, "کد تخفیف حذف شد"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در حذف کد تخفیف");
            return StatusCode(500, ApiResponse<CartSummaryDto>.Fail("خطا در حذف کد تخفیف"));
        }
    }

    #endregion

    #region AddToCart

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] Guid courseId)
    {
        var request = new AddCartItemRequest(courseId);
        if (request.CourseId == Guid.Empty)
            return Json(new { isSuccess = false, message = "شناسه دوره نامعتبر است." });

        var result = await cartService.AddCartItem(request);

        if (!result.IsSuccess)
        {
            var error = result.Errors?.FirstOrDefault()
                        ?? "خطا در افزودن دوره به سبد خرید.";
            return Json(new { isSuccess = false, message = error });
        }

        return Json(new { isSuccess = true, data = result.Data });
    }

    #endregion

    #region RemoveFromCart

    [HttpDelete("remove/{itemId:guid}")]
    public async Task<IActionResult> RemoveFromCart(Guid itemId)
    {
        var result = await cartService.RemoveCartItem(itemId);

        if (!result.IsSuccess)
        {
            var error = result.Errors?.FirstOrDefault()
                        ?? "خطا در حذف آیتم از سبد خرید.";
            return Json(new { isSuccess = false, message = error });
        }

        return Json(new { isSuccess = true, message = "آیتم با موفقیت حذف شد." });
    }

    #endregion

    #region ClearCart

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var result = await cartService.ClearCart();

        if (!result.IsSuccess)
        {
            var error = result.Errors?.FirstOrDefault()
                        ?? "خطا در خالی کردن سبد خرید.";
            return Json(new { isSuccess = false, message = error });
        }

        return Json(new { isSuccess = true, message = "سبد خرید خالی شد." });
    }

    #endregion

    #region Checkout

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var result = await cartService.CheckoutCart();

        if (!result.IsSuccess)
        {
            var error = result.Errors?.FirstOrDefault()
                        ?? "خطا در ثبت سفارش.";
            return Json(new { isSuccess = false, message = error });
        }

        return Json(new { isSuccess = true, data = result.Data });
    }

    #endregion

    #region CheckCourseInCart

    [HttpGet("check/{courseId:guid}")]
    public async Task<IActionResult> CheckCourseInCart(Guid courseId)
    {
        var result = await cartService.CheckCourseInCart(courseId);

        if (!result.IsSuccess)
            return Json(new { isSuccess = false, exists = false });

        return Json(new { isSuccess = true, exists = result.Data });
    }

    #endregion

    #region GetCartItemCount

    [HttpGet("count")]
    public async Task<IActionResult> GetCartItemCount()
    {
        var result = await cartService.GetCartItemCount();

        if (!result.IsSuccess)
            return Json(new { isSuccess = false, data = 0 });

        return Json(new { isSuccess = true, data = result.Data });
    }

    #endregion

    #region Private methods


    private static CartViewModel CreateEmptyViewModel()
    {
        return new CartViewModel
        {
            Cart = new CartDto
            {
                Items = [],
                ItemsTotal = 0,
                ItemsCount = 0,
                Currency = "تومان"
            },
            Summary = new CartSummaryDto
            {
                Subtotal = 0,
                Total = 0,
                Currency = "تومان"
            }
        };
    }
    private static CartSummaryDto BuildSummary(
        CartDto cart,
        string? couponCode,
        DiscountCalculationDto? discountResult)
    {
        var subtotal = cart.ItemsTotal;
        var discountAmount = discountResult?.DiscountAmount ?? 0m;

        // Derive discount percentage from amounts
        var discountPercentage = subtotal > 0 && discountAmount > 0
            ? (discountAmount / subtotal) * 100m
            : 0m;

        // Shipping: free above threshold (calculated on subtotal before discount)
        var shippingCost = subtotal >= FreeShippingThreshold
            ? 0m
            : StandardShippingCost;

        // Taxable amount = subtotal minus discount
        var taxableAmount = subtotal - discountAmount;

        // Tax & VAT on taxable amount
        var taxAmount = taxableAmount * TaxRate;
        var vatAmount = taxableAmount * VatRate;

        // Final total
        var total = taxableAmount + shippingCost + taxAmount + vatAmount;

        return new CartSummaryDto
        {
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            DiscountPercentage = Math.Round(discountPercentage, 2),
            ShippingCost = shippingCost,
            TaxAmount = Math.Round(taxAmount, 0),
            VatAmount = Math.Round(vatAmount, 0),
            Total = Math.Round(total, 0),
            Currency = cart.Currency ?? "IRT",
            AppliedCouponCode = couponCode
        };
    }
    #endregion
}
