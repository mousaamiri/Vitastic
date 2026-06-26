using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.CompleteOrder;

public sealed class CompleteOrderCommandHandler(
    IOrderRepository orderRepository,
    IPaymentTransactionRepository paymentTransactionRepository,
    ILogger<CompleteOrderCommandHandler> logger)
    : ICommandHandler<CompleteOrderCommand>
{
    public async Task<Result> Handle(
        CompleteOrderCommand request,
        CancellationToken cancellationToken)
    {
        #region Step 1: Validate and fetch Order

        Result<OrderId> orderIdResult = OrderId.CreateFrom(request.OrderId);
        if (orderIdResult.IsFailure)
            return orderIdResult.Error;

        Order? order = await orderRepository
            .FindAsync(orderIdResult.Value, cancellationToken);

        if (order is null)
            return Error.NotFound(
                "CompleteOrder.OrderNotFound",
                "سفارشی با این شناسه یافت نشد.");

        #region Idempotency Check - Already completed? Just return success

        if (order.Status == OrderStatus.Completed)
        {
            // Already completed from a previous retry — not an error
            logger.LogInformation(
                "سفارش {OrderId} قبلاً تکمیل شده — اجرای تکراری نادیده گرفته شد",
                request.OrderId);

            return Result.Success();
        }

        #endregion

        #endregion

        #region Step 2: Fetch and validate PaymentTransaction

        var transactionIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
        if (transactionIdResult.IsFailure)
            return transactionIdResult.Error;
        // We need transaction status & orderId for domain validation
        PaymentTransaction? transaction = await paymentTransactionRepository
            .FindAsync(transactionIdResult.Value, cancellationToken);

        if (transaction is null || transaction.Status != TransactionStatus.Completed)
        {
            logger.LogError(
                "تراکنش {TransactionId} روی سفارش {OrderId} وجود نداره — داده ناسازگار!",
                order.PaymentTransactionId, order.Id);

            return Error.NotFound(
                "CompleteOrder.TransactionNotFound",
                "تراکنش مربوط به این سفارش یافت نشد.");
        }
        #endregion

        #region Step 3: Domain — Complete order (+ GrantAccess internally)

        Result completeResult = order.Complete(
            transaction.Id,
            transaction.OrderId,
            transaction.Status);

        if (completeResult.IsFailure)
            return completeResult.Error;

        #endregion

        #region Step 4: Persist

        await orderRepository.UpdateAsync(order, cancellationToken);

        logger.LogInformation(
            "سفارش {OrderId} تکمیل شد — دسترسی به {ItemCount} آیتم اعطا شد",
            order.Id, order.Items.Count);

        #endregion

        return Result.Success();
    }
}
