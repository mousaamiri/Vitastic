using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Transaction;

namespace Vitastic.Web.Infrastructure.Services;

public interface ITransactionService
{
    Task<ApiResponse<InitializePaymentDto>> InitializePaymentAsync(InitializePaymentRequest request
        ,CancellationToken ct = default);
    Task<ApiResponse<PaymentVerificationDto>> VerifyPaymentAsync(VerifyAndCompletePaymentRequest request
        , CancellationToken ct = default);
}

public class TransactionService(IApiClient apiClient,ILogger<TransactionService> logger) : ITransactionService
{

    #region InitializePaymentAsync

    public async Task<ApiResponse<InitializePaymentDto>> InitializePaymentAsync(InitializePaymentRequest request,
        CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "در حال شروع پرداخت سفارش {OrderId} - مبلغ: {Amount}",
                request.OrderId, request.Amount);

            ApiResponse<InitializePaymentDto> response = await apiClient.PostAsync<InitializePaymentDto>(
                "payments/init",
                request,
                ct);

            if (!response.IsSuccess || response.Data is null)
            {
                logger.LogWarning("خطا در شروع پرداخت سفارش {OrderId}: {Error}",
                    request.OrderId, response.Message);
                return response;
            }

            logger.LogInformation(
                "پرداخت سفارش {OrderId} آماده شد - Authority: {Authority}, PaymentUrl: {Url}",
                request.OrderId, response.Data.Authority, response.Data.PaymentUrl);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در شروع پرداخت سفارش {OrderId}", request.OrderId);
            return new ApiResponse<InitializePaymentDto>(){Message = ex.Message};
        }
    }

    #endregion

    #region VerifyPaymentAsync
    public async Task<ApiResponse<PaymentVerificationDto>> VerifyPaymentAsync(VerifyAndCompletePaymentRequest request, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "در حال بررسی پرداخت - Authority: {Authority}, Status: {Status}",
                request.Authority,request.Status);

            ApiResponse<PaymentVerificationDto> response = await apiClient.PostAsync<PaymentVerificationDto>(
                "payments/verify",
                request,
                ct);

            if (!response.IsSuccess || response.Data is null)
            {
                logger.LogWarning("خطا در تأیید پرداخت - Authority: {Authority}: {Error}",
                    request.Authority, response.Message);
                return response;
            }

            if (!response.Data.IsSuccess)
            {
                logger.LogWarning(
                    "پرداخت ناموفق - Authority: {Authority}, TxId: {TxId}",
                    request.Authority, response.Data.TransactionId);
                return response;
            }

            logger.LogInformation(
                "پرداخت تأیید شد - TxId: {TxId}, RefId: {RefId}",
                response.Data.TransactionId, response.Data.RefId);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در تأیید پرداخت - Authority: {Authority}", request.Authority);
            return new ApiResponse<PaymentVerificationDto>(){Message = ex.Message};
        }
    }

    #endregion

}
