using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Wallets.Requests;
using Vitastic.Api.Features.Wallets.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Commands.AddFunds;
using Vitastic.App.Features.Wallets.Commands.CompleteWalletTransaction;
using Vitastic.App.Features.Wallets.Commands.Create;
using Vitastic.App.Features.Wallets.Commands.RevertWalletTransaction;
using Vitastic.App.Features.Wallets.Commands.WithdrawFunds;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.App.Features.Wallets.Queries.CheckSufficient;
using Vitastic.App.Features.Wallets.Queries.GetBalance;
using Vitastic.App.Features.Wallets.Queries.GetById;
using Vitastic.App.Features.Wallets.Queries.GetByUserId;
using Vitastic.App.Features.Wallets.Queries.GetUserTransactions;
using Vitastic.App.Features.Wallets.Queries.Search;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Wallets;

[Authorize]
[ApiController]
[Route("api/v1/wallets")]
[Produces("application/json")]
public class WalletsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<WalletsController> logger) : ControllerBase
{
    // ══════════════════════════════════════════════
    //  COMMANDS
    // ══════════════════════════════════════════════

    #region ==================== CREATE WALLET ====================

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> CreateWallet(
        [FromBody] CreateWalletRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating wallet — UserId: {UserId}",
            request.UserId);

        var command = new CreateWalletCommand(request.UserId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create wallet failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                request.UserId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("ایجاد کیف پول انجام نشد.");
        }

        logger.LogInformation(
            "Wallet created — WalletId: {WalletId}, UserId: {UserId}",
            result.Value, request.UserId);

        return result.ToApiResponse(
            t => t,
            "کیف پول با موفقیت ایجاد شد."
        );
    }

    #endregion


    #region ==================== ADD FUNDS ====================

    [HttpPost("{walletId:guid}/transactions/deposits")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> AddFunds(
        [FromRoute] Guid walletId,
        [FromBody] AddFundsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Adding funds — WalletId: {WalletId}, Amount: {Amount}",
            walletId, request.Amount);

        var command = new AddFundsCommand(walletId, request.Amount, request.Description);
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Add funds failed — WalletId: {WalletId}, {ErrorCode}: {ErrorMessage}",
                walletId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("واریز انجام نشد.");
        }

        logger.LogInformation(
            "Funds added — WalletId: {WalletId}, TransactionId: {TransactionId}, Amount: {Amount}",
            walletId, result.Value, request.Amount);

        return result.ToApiResponse(
            t => t,
            "واریز با موفقیت انجام شد."
        );
    }

    #endregion


    #region ==================== WITHDRAW FUNDS ====================

    [HttpPost("{walletId:guid}/transactions/withdrawals")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> WithdrawFunds(
        [FromRoute] Guid walletId,
        [FromBody] WithdrawFundsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Withdrawing funds — WalletId: {WalletId}, Amount: {Amount}",
            walletId, request.Amount);

        var command = new WithdrawFundsCommand(walletId, request.Amount, request.Description);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Withdrawal failed — WalletId: {WalletId}, {ErrorCode}: {ErrorMessage}",
                walletId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("برداشت انجام نشد.");
        }

        logger.LogInformation(
            "Funds withdrawn — WalletId: {WalletId}, TransactionId: {TransactionId}, Amount: {Amount}",
            walletId, result.Value, request.Amount);

        return result.ToApiResponse(
            t => t,
            "برداشت با موفقیت انجام شد."
        );
    }

    #endregion


    #region ==================== COMPLETE TRANSACTION ====================

    [HttpPatch("{walletId:guid}/transactions/{transactionId:guid}/completion")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> CompleteTransaction(
        [FromRoute] Guid walletId,
        [FromRoute] Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Completing transaction — WalletId: {WalletId}, TransactionId: {TransactionId}",
            walletId, transactionId);

        var command = new CompleteWalletTransactionCommand(walletId, transactionId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Complete transaction failed — TransactionId: {TransactionId}, {ErrorCode}: {ErrorMessage}",
                transactionId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("تکمیل تراکنش انجام نشد.");
        }

        logger.LogInformation(
            "Transaction completed — WalletId: {WalletId}, TransactionId: {TransactionId}",
            walletId, transactionId);

        return result.ToApiResponse("تراکنش با موفقیت تکمیل شد.");
    }

    #endregion


    #region ==================== REVERT TRANSACTION ====================

    [HttpPatch("{walletId:guid}/transactions/{transactionId:guid}/reversion")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> RevertTransaction(
        [FromRoute] Guid walletId,
        [FromRoute] Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Reverting transaction — WalletId: {WalletId}, TransactionId: {TransactionId}",
            walletId, transactionId);

        var command = new RevertWalletTransactionCommand(walletId, transactionId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Revert transaction failed — TransactionId: {TransactionId}, {ErrorCode}: {ErrorMessage}",
                transactionId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("بازگردانی تراکنش انجام نشد.");
        }

        logger.LogInformation(
            "Transaction reverted — WalletId: {WalletId}, TransactionId: {TransactionId}",
            walletId, transactionId);

        return result.ToApiResponse("تراکنش با موفقیت بازگردانی شد.");
    }

    #endregion


    // ══════════════════════════════════════════════
    //  QUERIES
    // ══════════════════════════════════════════════

    #region ==================== SEARCH WALLETS ====================
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<WalletResponse>>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<PaginatedResponse<WalletResponse>>> SearchWallets(
        [FromQuery] string? term = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Searching wallets — Term: {Term}, Page: {PageNumber}", term, pageNumber);

        var query = new SearchWalletsQuery(term, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Wallet search failed — {ErrorCode}: {ErrorMessage}", result.Error.Code, result.Error.Message);
            return result.ToApiResponse<PaginatedResponse<WalletResponse>>(result.Error.Message);
        }

        logger.LogInformation("Wallets search successful — Count: {Count}", result.Value.Items.Count);

        return result.ToApiResponse(
            mapper.Map<PaginatedResult<WalletDto>, PaginatedResponse<WalletResponse>>,
            "اطلاعات کیف پول‌ها با موفقیت دریافت شد."
        );
    }
    #endregion

    #region ==================== GET BY ID ====================

    [HttpGet("{walletId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WalletResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<WalletResponse>> GetWalletById(
        [FromRoute] Guid walletId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wallet — WalletId: {WalletId}", walletId);

        var result = await mediator.Send(new GetWalletQuery(walletId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Wallet not found — WalletId: {WalletId}, {ErrorCode}: {ErrorMessage}",
                walletId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<WalletResponse>("کیف پول یافت نشد.");
        }

        logger.LogInformation("Wallet retrieved — WalletId: {WalletId}", walletId);

        return result.ToApiResponse(
            mapper.Map<WalletResponse>,
            "اطلاعات کیف پول با موفقیت دریافت شد."
        );
    }

    #endregion

    #region ==================== GET BY USER ID ====================

    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WalletResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<WalletResponse>> GetWalletByUserId(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wallet by user — UserId: {UserId}", userId);

        var result = await mediator.Send(new GetUserWalletQuery(userId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Wallet not found for user — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<WalletResponse>("کیف پول برای این کاربر یافت نشد.");
        }

        logger.LogInformation("Wallet retrieved for user — UserId: {UserId}", userId);

        return result.ToApiResponse(
            mapper.Map<WalletResponse>,
            "اطلاعات کیف پول کاربر با موفقیت دریافت شد."
        );
    }

    #endregion

    #region ==================== GET USER WALLET TRANSACTIONS  ====================

    [HttpGet("user/{userId:guid}/transactions")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<WalletBalanceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PaginatedResponse<WalletTransactionResponse>>> GetUserWalletTransactions(
        [FromRoute] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting user wallet transactions — WalletId: {UserId}", userId);

        Result<PaginatedResult<WalletTransactionDto>> result =
            await mediator.Send(new GetUserTransactionsQuery(userId, pageNumber, pageSize), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get user wallet transactions failed — WalletId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<PaginatedResponse<WalletTransactionResponse>>(
                "دریافت موجودی کیف پول انجام نشد.");
        }

        logger.LogInformation("Getting user wallet transactions retrieved — WalletId: {UserId}", userId);

        return result.ToApiResponse(
            mapper.Map<PaginatedResponse<WalletTransactionResponse>>,
            "موجودی کیف پول با موفقیت دریافت شد.");
    }

    #endregion

    #region ==================== CHECK SUFFICIENT BALANCE ====================

    [HttpGet("{walletId:guid}/balance/sufficiency")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<bool>> CheckSufficientBalance(
        [FromRoute] Guid walletId,
        [FromQuery] decimal amount,
        CancellationToken cancellationToken)
    {
        // validation
        if (amount <= 0)
        {
            logger.LogWarning("Invalid amount for sufficiency check: {Amount}", amount);

            return ApiResponse<bool>.Fail(
                "مقدار باید بیشتر از صفر باشد.",
                ErrorType.Validation,
                ["InvalidAmount"]
            );
        }

        logger.LogInformation(
            "Checking balance sufficiency — WalletId: {WalletId}, Amount: {Amount}",
            walletId, amount);

        var result = await mediator.Send(new CheckSufficientBalanceQuery(walletId, amount), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Sufficiency check failed — WalletId: {WalletId}, {ErrorCode}: {ErrorMessage}",
                walletId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<bool>("بررسی موجودی انجام نشد.");
        }

        logger.LogInformation(
            "Sufficiency check completed — WalletId: {WalletId}, IsSufficient: {IsSufficient}",
            walletId, result.Value);

        return result.ToApiResponse(
            t => t,
            result.Value
                ? "موجودی کافی است."
                : "موجودی کافی نیست."
        );
    }

    #endregion
}
