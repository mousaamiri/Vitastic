using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Disocunt;

namespace Vitastic.Web.Infrastructure.Services;

/// <summary>
/// Client-side service interface for Discount operations
/// </summary>
public interface IDiscountService
{

    Task<ApiResponse<Guid>> CreateFixedDiscountAsync(
        UpsertDiscountRequest request,
        CancellationToken ct = default);

    Task<ApiResponse<Guid>> CreatePercentageDiscountAsync(
        UpsertDiscountRequest request,
        CancellationToken ct = default);

    Task<ApiResponse> DeactivateDiscountAsync(
        Guid discountId,
        CancellationToken ct = default);

    Task<ApiResponse> ExtendDiscountAsync(
        Guid discountId,
        ExtendDiscountRequest request,
        CancellationToken ct = default);

    Task<ApiResponse> SetMaximumAmountAsync(
        Guid discountId,
        SetMaximumAmountRequest request,
        CancellationToken ct = default);

    Task<ApiResponse> SetMinimumAmountAsync(
        Guid discountId,
        SetMinimumAmountRequest request,
        CancellationToken ct = default);

    Task<ApiResponse<DiscountDetailDto>> GetDiscountByIdAsync(
        Guid discountId,
        CancellationToken ct = default);

    Task<ApiResponse<DiscountDto>> GetDiscountByCodeAsync(
        string code,
        CancellationToken ct = default);

    Task<ApiResponse<DiscountCalculationDto>> CalculateDiscountAsync(
        string discountCode,
        decimal orderAmount,
        CancellationToken ct = default);

    Task<ApiResponse<bool>> CheckDiscountValidityAsync(
        Guid discountId,
        Guid? userId = null,
        CancellationToken ct = default);

    Task<ApiResponse<PaginatedData<DiscountDto>>> ListDiscountsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    Task<ApiResponse> UpdateDiscountAsync(UpsertDiscountRequest modelDiscount, CancellationToken ct = default);
}

public class DiscountService(
    IApiClient apiClient,
    ILogger<DiscountService> logger) : IDiscountService
{

    private const string BaseRoute = "Discounts";

    public async Task<ApiResponse<Guid>> CreateFixedDiscountAsync(
        UpsertDiscountRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("ایجاد کد تخفیف ثابت - کد: {Code}", request.Code);

        var result = await apiClient.PostAsync<Guid>(
            $"{BaseRoute}/fixed",
            request,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در ایجاد کد تخفیف ثابت - کد: {Code}, خطا: {Error}",
                request.Code, result.Message);
        else
            logger.LogInformation("کد تخفیف ثابت با موفقیت ایجاد شد - شناسه: {Id}", result.Data);

        return result;
    }

    public async Task<ApiResponse<Guid>> CreatePercentageDiscountAsync(
        UpsertDiscountRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("ایجاد کد تخفیف درصدی - کد: {Code}", request.Code);

        var result = await apiClient.PostAsync<Guid>(
            $"{BaseRoute}/percentage",
            request,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در ایجاد کد تخفیف درصدی - کد: {Code}, خطا: {Error}",
                request.Code, result.Message);
        else
            logger.LogInformation("کد تخفیف درصدی با موفقیت ایجاد شد - شناسه: {Id}", result.Data);

        return result;
    }

    public async Task<ApiResponse> DeactivateDiscountAsync(
        Guid discountId,
        CancellationToken ct = default)
    {
        logger.LogInformation("غیرفعال‌سازی کد تخفیف - شناسه: {DiscountId}", discountId);

        var result = await apiClient.PatchAsync(
            $"{BaseRoute}/{discountId}/deactivate",
            ct: ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در غیرفعال‌سازی کد تخفیف - شناسه: {DiscountId}, خطا: {Error}",
                discountId, result.Message);
        else
            logger.LogInformation("کد تخفیف با موفقیت غیرفعال شد - شناسه: {DiscountId}", discountId);

        return result;
    }

    public async Task<ApiResponse> ExtendDiscountAsync(
        Guid discountId,
        ExtendDiscountRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("تمدید کد تخفیف - شناسه: {DiscountId}, تاریخ جدید: {EndDate}",
            discountId, request.EndDate);

        var result = await apiClient.PatchAsync(
            $"{BaseRoute}/{discountId}/extend",
            request,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در تمدید کد تخفیف - شناسه: {DiscountId}, خطا: {Error}",
                discountId, result.Message);
        else
            logger.LogInformation("کد تخفیف با موفقیت تمدید شد - شناسه: {DiscountId}", discountId);

        return result;
    }

    public async Task<ApiResponse> SetMaximumAmountAsync(
        Guid discountId,
        SetMaximumAmountRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("تنظیم حداکثر مبلغ تخفیف - شناسه: {DiscountId}, مبلغ: {MaxAmount}",
            discountId, request.MaxAmount);

        var result = await apiClient.PatchAsync(
            $"{BaseRoute}/{discountId}/maximum-amount",
            request,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در تنظیم حداکثر مبلغ تخفیف - شناسه: {DiscountId}, خطا: {Error}",
                discountId, result.Message);
        else
            logger.LogInformation("حداکثر مبلغ تخفیف با موفقیت تنظیم شد - شناسه: {DiscountId}", discountId);

        return result;
    }

    public async Task<ApiResponse> SetMinimumAmountAsync(
        Guid discountId,
        SetMinimumAmountRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("تنظیم حداقل مبلغ سفارش - شناسه: {DiscountId}, مبلغ: {MinAmount}",
            discountId, request.MinAmount);

        var result = await apiClient.PatchAsync(
            $"{BaseRoute}/{discountId}/minimum-amount",
            request,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در تنظیم حداقل مبلغ سفارش - شناسه: {DiscountId}, خطا: {Error}",
                discountId, result.Message);
        else
            logger.LogInformation("حداقل مبلغ سفارش با موفقیت تنظیم شد - شناسه: {DiscountId}", discountId);

        return result;
    }

    public async Task<ApiResponse<DiscountDetailDto>> GetDiscountByIdAsync(
        Guid discountId,
        CancellationToken ct = default)
    {
        logger.LogInformation("دریافت اطلاعات کد تخفیف - شناسه: {DiscountId}", discountId);

        var result = await apiClient.GetAsync<DiscountDetailDto>(
            $"{BaseRoute}/{discountId}",
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("کد تخفیف یافت نشد - شناسه: {DiscountId}", discountId);
        else
            logger.LogInformation("اطلاعات کد تخفیف دریافت شد - شناسه: {DiscountId}", discountId);

        return result;
    }

    public async Task<ApiResponse<DiscountDto>> GetDiscountByCodeAsync(
        string code,
        CancellationToken ct = default)
    {
        logger.LogInformation("دریافت اطلاعات کد تخفیف - کد: {Code}", code);

        var result = await apiClient.GetAsync<DiscountDto>(
            $"{BaseRoute}/code/{code}",
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("کد تخفیف یافت نشد - کد: {Code}", code);
        else
            logger.LogInformation("اطلاعات کد تخفیف دریافت شد - کد: {Code}", code);

        return result;
    }

    public async Task<ApiResponse<DiscountCalculationDto>> CalculateDiscountAsync(
        string discountCode,
        decimal orderAmount,
        CancellationToken ct = default)
    {
        logger.LogInformation("محاسبه تخفیف - شناسه: {DiscountCode}, مبلغ سفارش: {OrderAmount}",
            discountCode, orderAmount);

        var result = await apiClient.GetAsync<DiscountCalculationDto>(
            $"{BaseRoute}/{discountCode}/calculate",
            new { orderAmount },
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در محاسبه تخفیف - شناسه: {DiscountCode}, خطا: {Error}",
                discountCode, result.Message);
        else
            logger.LogInformation("تخفیف محاسبه شد - شناسه: {DiscountCode}, مبلغ تخفیف: {Amount}",
                discountCode, result.Data?.DiscountAmount);

        return result;
    }

    public async Task<ApiResponse<bool>> CheckDiscountValidityAsync(
        Guid discountId,
        Guid? userId = null,
        CancellationToken ct = default)
    {
        logger.LogInformation("بررسی اعتبار کد تخفیف - شناسه: {DiscountId}, کاربر: {UserId}",
            discountId, userId);

        var queryParams = userId.HasValue
            ? new { userId = userId.Value } as object
            : null;

        var result = await apiClient.GetAsync<bool>(
            $"{BaseRoute}/{discountId}/validity",
            queryParams,
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در بررسی اعتبار کد تخفیف - شناسه: {DiscountId}, خطا: {Error}",
                discountId, result.Message);
        else
            logger.LogInformation("اعتبار کد تخفیف بررسی شد - شناسه: {DiscountId}, معتبر: {IsValid}",
                discountId, result.Data);

        return result;
    }

    public async Task<ApiResponse<PaginatedData<DiscountDto>>> ListDiscountsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        logger.LogInformation("دریافت لیست تخفیف‌ها - صفحه: {Page}, تعداد: {Size}",
            pageNumber, pageSize);

        var result = await apiClient.GetAsync<PaginatedData<DiscountDto>>(
            BaseRoute,
            new { pageNumber, pageSize },
            ct);

        if (!result.IsSuccess)
            logger.LogWarning("خطا در دریافت لیست تخفیف‌ها - خطا: {Error}", result.Message);
        else
            logger.LogInformation("لیست تخفیف‌ها دریافت شد - تعداد: {Count}",
                result.Data?.Items?.Count ?? 0);

        return result;
    }

    public async Task<ApiResponse> UpdateDiscountAsync(UpsertDiscountRequest modelDiscount, CancellationToken ct = default)
    => await apiClient.PutAsync(BaseRoute, modelDiscount, ct);
}
