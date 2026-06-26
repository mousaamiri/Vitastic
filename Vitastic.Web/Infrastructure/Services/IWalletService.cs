using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Transaction;
using Vitastic.Web.Models.DTOs.Wallet;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Infrastructure.Services;

public interface IWalletService
{
    // Main orchestration methods
    Task<ApiResponse<WalletInfoModel>> InitWalletAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);

    Task<ApiResponse<string>> ChargeWalletAsync(
        Guid walletId, AddFundsRequest request, CancellationToken ct = default);

    // Granular methods
    Task<ApiResponse<WalletDto>> GetWalletByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<ApiResponse<WalletBalanceDto>> GetWalletBalanceAsync(Guid walletId, CancellationToken ct = default);

    Task<ApiResponse<bool>> CheckSufficientBalanceAsync(
        Guid walletId, decimal amount, CancellationToken ct = default);

    Task<ApiResponse<PaginatedData<WalletTransactionDto>>> GetUserTransactionsAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken ct = default);

    Task<ApiResponse<Guid>> AddFundsAsync(
        Guid walletId, AddFundsRequest request, CancellationToken ct = default);

    Task<ApiResponse> VerifyWalletChargeAsync(
        string authority, string status, string? refId, CancellationToken ct = default);

    Task<ApiResponse<PaginatedData<WalletDto>>> GetUsersWallets(int pageNumber=1, int pageSize=10, CancellationToken ct=default);
}



internal sealed class WalletService(
    IApiClient apiClient,
    ILogger<WalletService> logger,
    ITransactionService transactionService) : IWalletService
{
    #region GetWalletByUserIdAsync

    public async Task<ApiResponse<WalletDto>> GetWalletByUserIdAsync(
        Guid userId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال دریافت کیف پول کاربر {UserId}", userId);

            var response = await apiClient.GetAsync<WalletDto>(
                $"wallets/user/{userId}", ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در دریافت کیف پول کاربر {UserId}: {Error}",
                    userId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت کیف پول کاربر {UserId}", userId);
            return new ApiResponse<WalletDto> { Message = ex.Message };
        }
    }

    #endregion

    #region GetWalletBalanceAsync

    public async Task<ApiResponse<WalletBalanceDto>> GetWalletBalanceAsync(
        Guid walletId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال دریافت موجودی کیف پول {WalletId}", walletId);

            var response = await apiClient.GetAsync<WalletBalanceDto>(
                $"wallets/{walletId}/balance", ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در دریافت موجودی کیف پول {WalletId}: {Error}",
                    walletId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت موجودی کیف پول {WalletId}", walletId);
            return new ApiResponse<WalletBalanceDto> { Message = ex.Message };
        }
    }

    #endregion

    #region GetUserTransactionsAsync

    public async Task<ApiResponse<PaginatedData<WalletTransactionDto>>> GetUserTransactionsAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "در حال دریافت تراکنش‌های کاربر {UserId} - صفحه {PageNumber}",
                userId, pageNumber);

            var response = await apiClient.GetAsync<PaginatedData<WalletTransactionDto>>(
                $"wallets/user/{userId}/transactions?pageNumber={pageNumber}&pageSize={pageSize}", ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در دریافت تراکنش‌های کاربر {UserId}: {Error}",
                    userId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت تراکنش‌های کاربر {UserId}", userId);
            return new ApiResponse<PaginatedData<WalletTransactionDto>> { Message = ex.Message };
        }
    }

    #endregion

    #region CheckSufficientBalanceAsync

    public async Task<ApiResponse<bool>> CheckSufficientBalanceAsync(
        Guid walletId, decimal amount, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "در حال بررسی موجودی کافی کیف پول {WalletId} برای مبلغ {Amount}",
                walletId, amount);

            var response = await apiClient.GetAsync<bool>(
                $"wallets/{walletId}/balance/sufficiency?amount={amount}", ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در بررسی موجودی کیف پول {WalletId}: {Error}",
                    walletId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در بررسی موجودی کیف پول {WalletId}", walletId);
            return new ApiResponse<bool> { Message = ex.Message };
        }
    }

    #endregion

    #region AddFundsAsync

    public async Task<ApiResponse<Guid>> AddFundsAsync(
        Guid walletId, AddFundsRequest request, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "در حال ثبت تراکنش واریز به کیف پول {WalletId} با مبلغ {Amount}",
                walletId, request.Amount);

            var response = await apiClient.PostAsync<Guid>(
                $"wallets/{walletId}/transactions/deposits",
                request, ct);

            if (!response.IsSuccess)
            {
                logger.LogWarning("خطا در ثبت تراکنش واریز به کیف پول {WalletId}: {Error}",
                    walletId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ثبت تراکنش واریز به کیف پول {WalletId}", walletId);
            return new ApiResponse<Guid> { Message = ex.Message };
        }
    }

    #endregion

    #region InitWalletAsync (Orchestrator)

    public async Task<ApiResponse<WalletInfoModel>> InitWalletAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("در حال مقداردهی اولیه کیف پول کاربر {UserId}", userId);

            // Step 1: Get wallet info
            var walletResponse = await GetWalletByUserIdAsync(userId, ct);
            if (!walletResponse.IsSuccess || walletResponse.Data is null)
            {
                logger.LogWarning("کیف پول کاربر {UserId} یافت نشد", userId);
                return new ApiResponse<WalletInfoModel>
                {
                    Message = walletResponse.Message ?? "کیف پول یافت نشد"
                };
            }

            // Step 2: Get transactions (NON-CRITICAL)
            var transactionsResponse = await GetUserTransactionsAsync(
                userId, pageNumber, pageSize, ct);

            var transactions = transactionsResponse is { IsSuccess: true, Data: not null }
                ? transactionsResponse.Data
                : new PaginatedData<WalletTransactionDto>();

            // Step 3: Build view model
            var model = new WalletInfoModel
            {
                WalletDto = walletResponse.Data,
                ChargeRequest = new AddFundsRequest(),
                Transactions = transactions,
                CallbackUrl = string.Empty // Will be set in controller
            };

            logger.LogInformation("مقداردهی کیف پول کاربر {UserId} با موفقیت انجام شد", userId);

            return new ApiResponse<WalletInfoModel>
            {
                Data = model,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در مقداردهی کیف پول کاربر {UserId}", userId);
            return new ApiResponse<WalletInfoModel> { Message = ex.Message };
        }
    }

    #endregion

    #region ChargeWalletAsync (Orchestrator)

    public async Task<ApiResponse<string>> ChargeWalletAsync(
        Guid WalletId, AddFundsRequest request, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "شروع فرآیند شارژ کیف پول  {WalletId} با مبلغ {Amount}",
                WalletId, request.Amount);


            var paymentRequest = new InitializePaymentRequest(
                Amount: request.Amount,
                TransactionType: TransactionTypeRequest.Deposit,
                WalletId: WalletId,
                CallbackUrl: request.CallbackUrl
            );

            var paymentResult =
                await transactionService.InitializePaymentAsync(paymentRequest, ct);

            if (!paymentResult.IsSuccess || paymentResult.Data is null)
            {
                return new ApiResponse<string>
                {
                    Message = paymentResult.Message ?? "خطا در شروع فرآیند پرداخت"
                };
            }

            logger.LogInformation(
                "فرآیند شارژ کیف پول  {WalletId} تکمیل شد. انتقال به درگاه: {Url}",
                WalletId, paymentResult.Data.PaymentUrl);

            return new ApiResponse<string>
            {
                Data = paymentResult.Data.PaymentUrl,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در فرآیند شارژ کیف پول  {WalletId}", WalletId);
            return new ApiResponse<string> { Message = ex.Message };
        }
    }

    #endregion
    #region VerifyWalletChargeAsync (Orchestrator)

    public async Task<ApiResponse> VerifyWalletChargeAsync(
        string authority, string status, string? refId, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation(
                "شروع فرآیند تایید شارژ کیف پول - Authority: {Authority}, Status: {Status}",
                authority, status);

            // Step 1: Verify payment with gateway
            var verifyRequest = new VerifyAndCompletePaymentRequest(
                Authority: authority,
                Status: status,
                CallbackRefId: refId
            );

            var verifyResult = await transactionService.VerifyPaymentAsync(verifyRequest, ct);

            if (!verifyResult.IsSuccess)
            {
                logger.LogWarning(
                    "تایید پرداخت ناموفق بود - Authority: {Authority}, Error: {Error}",
                    authority, verifyResult.Message);

                return new ApiResponse { Message = verifyResult.Message };
            }

            logger.LogInformation(
                "شارژ کیف پول با موفقیت تایید شد - Authority: {Authority}, RefId: {RefId}",
                authority, verifyResult.Data?.RefId);

            return new ApiResponse { IsSuccess = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در فرآیند تایید شارژ کیف پول - Authority: {Authority}", authority);
            return new ApiResponse { Message = ex.Message };
        }
    }



    #endregion

    #region Get List of wallets with search

    public async Task<ApiResponse<PaginatedData<WalletDto>>> GetUsersWallets(int pageNumber = 1, int pageSize = 10,
        CancellationToken ct = default)
        => await apiClient.GetPaginatedAsync<WalletDto>("wallets", new { pageNumber, pageSize }, ct);

    #endregion

}
