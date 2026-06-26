using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Payments.Requests;
using Vitastic.Api.Features.Payments.Responses;
using Vitastic.App.Features.Payments.Commands.AssignInfo;
using Vitastic.App.Features.Payments.Commands.AssignPaymentToOrder;
using Vitastic.App.Features.Payments.Commands.Cancel;
using Vitastic.App.Features.Payments.Commands.Create;
using Vitastic.App.Features.Payments.Commands.Fail;
using Vitastic.App.Features.Payments.Commands.Init;
using Vitastic.App.Features.Payments.Commands.Revert;
using Vitastic.App.Features.Payments.Commands.Verify;
using Vitastic.App.Features.Payments.Queries.GetById;
using Vitastic.App.Features.Payments.Queries.GetByStatus;

namespace Vitastic.Api.Features.Payments;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PaymentsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<PaymentsController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================
    #region Create Payment

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> CreatePayment(
        [FromBody] CreatePaymentTransactionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating payment - WalletId: {WalletId}", request.WalletId);

        var command = mapper.Map<CreatePaymentTransactionCommand>(request);
        var result = await mediator.Send(command, cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create payment failed - {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("خطا در ایجاد تراکنش پرداخت");
        }

        logger.LogInformation("Payment created - Id: {PaymentId}", result.Value);

        return result
            .ToApiResponse(t => t, "تراکنش پرداخت با موفقیت ایجاد شد");
    }

    #endregion



    #region Init Payment

    [HttpPost("init")]
    [ProducesResponseType(typeof(ApiResponse<InitializePaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<InitializePaymentResponse>> InitPayment(
        [FromBody] InitializePaymentRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Initializing payment - WalletId: {WalletId}, OrderId: {OrderId}",
            request.WalletId, request.OrderId);

        var command = mapper.Map<InitializePaymentCommand>(request);

        // Set callback URL if missing
        if (request.CallbackUrl is null)
        {
            var verifyUrl = Url.Action(
                action: nameof(VerifyPayment),
                controller: "Payments",
                values: null,
                protocol: HttpContext.Request.Scheme);

            command = command with { CallbackUrl = verifyUrl };
        }

        var result = await mediator.Send(command, cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Initialize payment failed - {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<InitializePaymentResponse>("خطا در آماده‌سازی پرداخت");
        }


        logger.LogInformation(
            "Payment initialized - TransactionId: {TransactionId}, Authority: {Authority}",
            result.Value.TransactionId, result.Value.Authority);

        return result
            .ToApiResponse(mapper.Map<InitializePaymentResponse>, "پرداخت با موفقیت آماده‌سازی شد");
    }

    #endregion

    #region Verify Payment

    [HttpPost("verify")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaymentVerificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<PaymentVerificationResponse>> VerifyPayment(
        [FromBody] VerifyAndCompletePaymentRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Verifying payment - Authority: {Authority}", request.Authority);

        var command = mapper.Map<VerifyAndCompletePaymentCommand>(request);
        var result = await mediator.Send(command, cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Verify payment failed - Authority: {Authority}, Error: {ErrorCode}",
                request.Authority, result.Error.Code);

            return result.ToApiResponse<PaymentVerificationResponse>("خطا در تایید پرداخت");
        }

        var response = mapper.Map<PaymentVerificationResponse>(result.Value);

        logger.LogInformation(
            "Payment verified - TransactionId: {TransactionId}, IsSuccess: {IsSuccess}, RefId: {RefId}",
            response.TransactionId, response.IsSuccess, response.RefId);

        return result
            .ToApiResponse(mapper.Map<PaymentVerificationResponse>, "پرداخت با موفقیت تایید شد");
    }

    #endregion



    #region Mark Payment Failed

    [HttpPatch("{transactionId:guid}/status/failed")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> MarkFailPayment(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking payment as failed - Id: {TransactionId}", transactionId);

        var result = await mediator.Send(
            new MarkPaymentFailedCommand(transactionId), cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Mark payment as failed failed - Id: {TransactionId}, Error: {ErrorCode}",
                transactionId, result.Error.Code);

            return result.ToApiResponse("خطا در علامت‌گذاری پرداخت به عنوان ناموفق");
        }

        logger.LogInformation("Payment marked as failed - Id: {TransactionId}", transactionId);

        return result.ToApiResponse("پرداخت به عنوان ناموفق علامت‌گذاری شد");
    }

    #endregion



    #region Cancel Payment

    [HttpPatch("{transactionId:guid}/status/cancel")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> CancelPayment(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling payment - Id: {TransactionId}", transactionId);

        var result = await mediator.Send(
            new CancelPaymentCommand(transactionId), cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Cancel payment failed - Id: {TransactionId}, Error: {ErrorCode}",
                transactionId, result.Error.Code);

            return result.ToApiResponse("خطا در لغو پرداخت");
        }

        logger.LogInformation("Payment cancelled - Id: {TransactionId}", transactionId);

        return result.ToApiResponse("پرداخت با موفقیت لغو شد");
    }

    #endregion
    #region Revert Payment

    [HttpPatch("{transactionId:guid}/revert")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> RevertPayment(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reverting payment - Id: {TransactionId}", transactionId);

        var result = await mediator.Send(
            new RevertPaymentCommand(transactionId), cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Revert payment failed - Id: {TransactionId}, Error: {ErrorCode}",
                transactionId, result.Error.Code);

            return result.ToApiResponse("خطا در بازگردانی پرداخت");
        }

        logger.LogInformation("Payment reverted - Id: {TransactionId}", transactionId);

        return result.ToApiResponse("پرداخت با موفقیت برگشت داده شد");
    }

    #endregion



    #region Assign Payment Info

    [HttpPatch("{transactionId:guid}/payment-info")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> AssignInfoPayment(
        Guid transactionId,
        [FromBody] AssignPaymentInfoRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Assigning payment info - Id: {TransactionId}, Authority: {Authority}",
            transactionId, request.Authority);

        // Map request to command
        var command = mapper.Map<AssignPaymentInfoCommand>(request) with
        {
            TransactionId = transactionId
        };

        var result = await mediator.Send(command, cancellationToken);


        if (result.IsFailure)
        {
            logger.LogWarning(
                "Assign payment info failed - Id: {TransactionId}, Error: {ErrorCode}",
                transactionId, result.Error.Code);

            return result.ToApiResponse("خطا در تخصیص اطلاعات پرداخت");
        }

        logger.LogInformation(
            "Payment info assigned - Id: {TransactionId}", transactionId);

        return result.ToApiResponse("اطلاعات پرداخت با موفقیت تخصیص داده شد");
    }

    #endregion



    #region Assign Payment To Order

    [HttpPatch("{transactionId:guid}/order/{orderId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> AssignPaymentToOrder(
        Guid transactionId,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Assigning payment to order - TransactionId: {TransactionId}, OrderId: {OrderId}",
            transactionId, orderId);

        var result = await mediator.Send(
            new AssignPaymentToOrderCommand(transactionId, orderId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Assign payment to order failed - TransactionId: {TransactionId}, Error: {ErrorCode}",
                transactionId, result.Error.Code);

            return result.ToApiResponse("خطا در تخصیص پرداخت به سفارش");
        }

        logger.LogInformation(
            "Payment assigned to order - TransactionId: {TransactionId}, OrderId: {OrderId}",
            transactionId, orderId);

        return result.ToApiResponse("پرداخت با موفقیت به سفارش تخصیص داده شد");
    }

    #endregion



    // ======================== QUERIES ========================
    #region Get Payment

    [HttpGet("{transactionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PaymentTransactionResponse>> GetPayment(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting payment - Id: {TransactionId}", transactionId);

        var result = await mediator.Send(
            new GetPaymentTransactionQuery(transactionId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Payment not found - Id: {TransactionId}", transactionId);

            return result.ToApiResponse<PaymentTransactionResponse>("تراکنش پرداخت یافت نشد");
        }

        logger.LogInformation("Payment retrieved - Id: {TransactionId}", transactionId);

        return result
            .ToApiResponse(mapper.Map<PaymentTransactionResponse>, "تراکنش پرداخت با موفقیت بازیابی شد");
    }

    #endregion



    #region Get Payment Status

    [HttpGet("{transactionId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<PaymentTransactionStatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PaymentTransactionStatusResponse>> GetPaymentStatus(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting payment status - Id: {TransactionId}", transactionId);

        var result = await mediator.Send(
            new GetPaymentTransactionStatusQuery(transactionId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Payment not found - Id: {TransactionId}", transactionId);

            return result.ToApiResponse<PaymentTransactionStatusResponse>("تراکنش پرداخت یافت نشد");
        }

        var response = mapper.Map<PaymentTransactionStatusResponse>(result.Value);

        logger.LogInformation(
            "Payment status retrieved - Id: {TransactionId}, Status: {Status}",
            transactionId, response.Status);

        return result
            .ToApiResponse(mapper.Map<PaymentTransactionStatusResponse>, "وضعیت پرداخت با موفقیت بازیابی شد");
    }

    #endregion


}
