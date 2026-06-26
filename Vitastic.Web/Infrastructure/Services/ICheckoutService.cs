using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Checkout;
using Vitastic.Web.Models.DTOs.Order;
using Vitastic.Web.Models.DTOs.Transaction;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Infrastructure.Services;

#region Interface

public interface ICheckoutService
{
    // Main orchestration methods
    Task<ApiResponse<CheckoutViewModel>> GetCheckoutDataAsync(Guid userId, CancellationToken ct = default);

    Task<ApiResponse<Guid>> CreateOrderFromCartAsync(Guid userId, CancellationToken ct = default);

    Task<ApiResponse<string>> SubmitPaymentAsync(CheckoutSubmitDto dto, Guid userId, CancellationToken ct = default);

    // Granular methods
    Task<ApiResponse> UpdateCustomerNoteAsync(Guid orderId, string note, CancellationToken ct = default);

    Task<ApiResponse> UpdateUserProfileAsync(
        Guid userId, string firstName, string lastName, string phoneNumber, CancellationToken ct = default);

    Task<ApiResponse> AssignCustomerInfoToOrderAsync(
        Guid orderId, string customerFullName, CancellationToken ct = default);

    Task<ApiResponse> CompleteOrderAsync(Guid orderId, CancellationToken ct = default);
}

#endregion

#region Implementation

public class CheckoutService(
    IApiClient apiClient,
    ITransactionService transactionService,
    ILogger<CheckoutService> logger) : ICheckoutService
{
    #region GetCheckoutDataAsync

    public async Task<ApiResponse<CheckoutViewModel>> GetCheckoutDataAsync(
        Guid userId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال دریافت اطلاعات چک‌اوت برای کاربر {UserId}", userId);

            // Step 1: Create order from cart
            var orderCreationResponse = await CreateOrderFromCartAsync(userId, ct);
            if (!orderCreationResponse.IsSuccess || orderCreationResponse.Data == Guid.Empty)
            {
                logger.LogWarning("خطا در ایجاد سفارش از سبد: {Error}", orderCreationResponse.Message);
                return new ApiResponse<CheckoutViewModel> { Message = orderCreationResponse.Message };
            }

            var orderId = orderCreationResponse.Data;

            // Step 2: Get order details
            var orderResponse = await apiClient.GetAsync<OrderDetailApiDto>($"orders/{orderId}", ct);

            if (!orderResponse.IsSuccess || orderResponse.Data is null)
            {
                logger.LogWarning("سفارش {OrderId} یافت نشد", orderId);
                return new ApiResponse<CheckoutViewModel> { Message = orderResponse.Message ?? "سفارش یافت نشد" };
            }

            // Step 3: Build view model
            var viewModel = new CheckoutViewModel
            {
                Order = orderResponse.Data,
                PaymentMethods = GetAvailablePaymentMethods()
            };

            logger.LogInformation("اطلاعات چک‌اوت برای سفارش {OrderId} آماده شد", orderId);

            return new ApiResponse<CheckoutViewModel>
            {
                Data = viewModel,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت اطلاعات چک‌اوت برای کاربر {UserId}", userId);
            return new ApiResponse<CheckoutViewModel> { Message = ex.Message };
        }
    }

    #endregion

    #region CreateOrderFromCartAsync

    public async Task<ApiResponse<Guid>> CreateOrderFromCartAsync(
        Guid userId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال ایجاد سفارش از سبد خرید کاربر {UserId}", userId);

            var response = await apiClient.PostAsync<Guid>(
                "orders",
                new { UserId = userId },
                ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در ایجاد سفارش برای کاربر {UserId}: {Error}",
                    userId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ایجاد سفارش برای کاربر {UserId}", userId);
            return new ApiResponse<Guid> { Message = ex.Message };
        }
    }

    #endregion

    #region UpdateCustomerNoteAsync

    public async Task<ApiResponse> UpdateCustomerNoteAsync(
        Guid orderId, string note, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال ذخیره یادداشت برای سفارش {OrderId}", orderId);

            var response = await apiClient.PatchAsync(
                $"orders/{orderId}/customer-notes",
                new AddCustomerNoteRequest(note.Trim()),
                ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در ذخیره یادداشت سفارش {OrderId}: {Error}",
                    orderId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ذخیره یادداشت سفارش {OrderId}", orderId);
            return new ApiResponse { Message = ex.Message };
        }
    }

    #endregion

    #region UpdateUserProfileAsync

    public async Task<ApiResponse> UpdateUserProfileAsync(
        Guid userId, string firstName, string lastName, string phoneNumber, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال بروزرسانی پروفایل کاربر {UserId}", userId);

            var response = await apiClient.PatchAsync(
                $"users/{userId}/profile",
                new
                {
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    PhoneNumber = phoneNumber.Trim()
                },
                ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در بروزرسانی پروفایل کاربر {UserId}: {Error}",
                    userId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در بروزرسانی پروفایل کاربر {UserId}", userId);
            return new ApiResponse { Message = ex.Message };
        }
    }

    #endregion

    #region AssignCustomerInfoToOrderAsync

    public async Task<ApiResponse> AssignCustomerInfoToOrderAsync(
        Guid orderId, string customerFullName, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال ثبت اطلاعات خریدار روی سفارش {OrderId}", orderId);

            var response = await apiClient.PatchAsync(
                $"orders/{orderId}/customer-information?customerFullName={Uri.EscapeDataString(customerFullName.Trim())}",
                ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در ثبت اطلاعات خریدار سفارش {OrderId}: {Error}",
                    orderId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ثبت اطلاعات خریدار سفارش {OrderId}", orderId);
            return new ApiResponse { Message = ex.Message };
        }
    }

    #endregion

    #region CompleteOrderAsync

    public async Task<ApiResponse> CompleteOrderAsync(
        Guid orderId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال تکمیل سفارش {OrderId}", orderId);

            var response = await apiClient.PatchAsync(
                $"orders/{orderId}/status/completed",
                ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در تکمیل سفارش {OrderId}: {Error}",
                    orderId, response.Message);
            }
            else
            {
                logger.LogInformation("سفارش {OrderId} با موفقیت تکمیل شد", orderId);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در تکمیل سفارش {OrderId}", orderId);
            return new ApiResponse { Message = ex.Message };
        }
    }

    #endregion

    #region SubmitPaymentAsync (Orchestrator)

    public async Task<ApiResponse<string>> SubmitPaymentAsync(
        CheckoutSubmitDto dto, Guid userId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("شروع فرآیند ثبت سفارش {OrderId} برای کاربر {UserId}",
                dto.OrderId, userId);

            // Step 1: Update user profile (NON-CRITICAL)
            if (!string.IsNullOrEmpty(dto.FirstName?.Trim())
                || !string.IsNullOrEmpty(dto.LastName?.Trim())
                || !string.IsNullOrEmpty(dto.PhoneNumber?.Trim()))
            {
                var profileResult = await UpdateUserProfileAsync(
                    userId, dto.FirstName ?? "", dto.LastName ?? "", dto.PhoneNumber, ct);

                if (!profileResult.IsSuccess)
                {
                    logger.LogWarning(
                        "بروزرسانی پروفایل کاربر {UserId} ناموفق بود: {Error}",
                        userId, profileResult.Message);
                }

                // Step 2: Assign customer full name to order (CRITICAL)
                if (!string.IsNullOrEmpty(dto.FirstName?.Trim()) &&
                    !string.IsNullOrEmpty(dto.LastName?.Trim()))
                {
                    var customerFullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}";
                    var assignResult = await AssignCustomerInfoToOrderAsync(
                        dto.OrderId, customerFullName, ct);

                    if (!assignResult.IsSuccess)
                    {
                        return new ApiResponse<string>
                        {
                            Message = assignResult.Message ?? "خطا در ثبت اطلاعات خریدار"
                        };
                    }
                }
            }

            // Step 3: Save customer note (NON-CRITICAL)
            if (!string.IsNullOrWhiteSpace(dto.CustomerNote))
            {
                var noteResult = await UpdateCustomerNoteAsync(
                    dto.OrderId, dto.CustomerNote, ct);

                if (!noteResult.IsSuccess)
                {
                    logger.LogWarning(
                        "ذخیره یادداشت سفارش {OrderId} ناموفق بود: {Error}",
                        dto.OrderId, noteResult.Message);
                }
            }

            // Step 4: Initialize payment (CRITICAL)
            var paymentRequest = new InitializePaymentRequest(
                Amount: dto.FinalAmount,
                TransactionType: TransactionTypeRequest.Deposit,
                OrderId: dto.OrderId,
                CallbackUrl: dto.CallbackUrl
            );

            var paymentResult = await transactionService.InitializePaymentAsync(paymentRequest, ct);

            if (!paymentResult.IsSuccess || paymentResult.Data is null)
            {
                return new ApiResponse<string>
                {
                    Message = paymentResult.Message ?? "خطا در شروع فرآیند پرداخت"
                };
            }

            logger.LogInformation(
                "فرآیند ثبت سفارش {OrderId} تکمیل شد. انتقال به درگاه: {Url}",
                dto.OrderId, paymentResult.Data.PaymentUrl);

            return new ApiResponse<string>
            {
                Data = paymentResult.Data.PaymentUrl,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در فرآیند ثبت سفارش {OrderId}", dto.OrderId);
            return new ApiResponse<string> { Message = ex.Message };
        }
    }

    #endregion

    #region Private Helpers

    private static List<PaymentMethodOption> GetAvailablePaymentMethods() =>
    [
        new()
        {
            Id = 1,
            Name = "درگاه بانکی",
            Icon = "bi-credit-card",
            Description = "پرداخت آنلاین از طریق درگاه بانکی",
            IsDefault = true
        },
        new()
        {
            Id = 2,
            Name = "کیف پول",
            Icon = "bi-wallet2",
            Description = "پرداخت از موجودی کیف پول"
        },
        new()
        {
            Id = 3,
            Name = "کد هدیه",
            Icon = "bi-gift",
            Description = "استفاده از کارت هدیه"
        }
    ];

    #endregion
}

#endregion
