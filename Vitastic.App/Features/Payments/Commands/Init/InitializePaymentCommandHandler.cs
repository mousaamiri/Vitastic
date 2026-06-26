using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Payments.Commands.Init;
public sealed class InitializePaymentCommandHandler(
    IPaymentTransactionRepository paymentRepository,
    IPaymentGatewayService gatewayService,
    IOrderRepository orderRepository,
    ILogger<InitializePaymentCommandHandler> logger)
    : ICommandHandler<InitializePaymentCommand, InitializePaymentResult>
{
    public async Task<Result<InitializePaymentResult>> Handle(
        InitializePaymentCommand command,
        CancellationToken cancellationToken)
    {
        #region Step 0: Validate — Ensure transaction has an owner

        // Each transaction must belong to either a Wallet or an Order
        if (!command.WalletId.HasValue && !command.OrderId.HasValue)
        {
            return Error.Validation(
                "InitPayment.NoOwner",
                "هر تراکنش باید متعلق به یک کیف پول یا یک سفارش باشد.");
        }

        #endregion

        #region Step 1: Validate — Check order is paidable (before calling gateway!)

        OrderId? orderId = null;

        if (command.OrderId.HasValue)
        {
            Result<OrderId> orderIdResult = OrderId.CreateFrom(command.OrderId.Value);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;

            orderId = orderIdResult.Value;

            // Check BEFORE hitting the gateway to avoid wasting API calls
            var isPaidable = await orderRepository
                .CheckIsPaidableAsync(orderId, cancellationToken);

            if (!isPaidable)
            {
                logger.LogWarning(
                    "سفارش {OrderId} قابل پرداخت نیست", command.OrderId);

                return Error.Conflict(
                    "InitPayment.OrderNotPaidable",
                    "این سفارش قابل پرداخت نیست. احتمالاً قبلاً پرداخت شده است.");
            }
        }

        #endregion

        #region Step 2: Domain — Create PaymentTransaction

        Money amount = Money.Create(command.Amount, Currency.IranianToman).Value;

        WalletId? walletId = command.WalletId.HasValue
            ? WalletId.CreateFrom(command.WalletId.Value).Value
            : null;

        Result<PaymentTransaction> transactionResult = PaymentTransaction.Create(
            amount,
            command.TransactionType,
            command.Description,
            walletId);

        if (transactionResult.IsFailure)
            return transactionResult.Error;

        PaymentTransaction transaction = transactionResult.Value;

        #endregion

        #region Step 3: Assign to order (if applicable)

        if (orderId is not null)
        {
            Result assignToOrderResult = transaction.AssignToOrder(orderId);
            if (assignToOrderResult.IsFailure)
                return assignToOrderResult.Error;

            // Assign transaction to order + move to Processing
            var order = await orderRepository.FindAsync(orderId, cancellationToken);
            if(order is null)
                return Error.NotFound("InitializePaymentCommand.OrderNotFound","سفارش یافت نشد.");
            Result processResult = order.ProcessPayment(
                transaction.Id,
                PaymentMethod.Online);

            if (processResult.IsFailure)
                return processResult.Error;
        }

        #endregion

        #region Step 4: Infrastructure — Request payment gateway

        Result<PaymentGatewayResult> gatewayResult = await gatewayService
            .CreatePaymentAsync(transaction, command.CallbackUrl ?? string.Empty);

        if (gatewayResult.IsFailure)
        {
            logger.LogError(
                "خطا در درگاه پرداخت برای تراکنش {TransactionId}: {Error}",
                transaction.Id.Value, gatewayResult.Error);

            return gatewayResult.Error;
        }

        PaymentGatewayResult gatewayData = gatewayResult.Value;

        #endregion

        #region Step 5: Domain — Assign gateway info to transaction

        Result assignResult = transaction.AssignPaymentInfo(
            gatewayData.Authority,
            "Zarinpal");

        if (assignResult.IsFailure)
            return assignResult.Error;

        #endregion

        #region Step 6: Persist

        await paymentRepository.AddAsync(transaction, cancellationToken);

        logger.LogInformation(
            "تراکنش پرداخت {TransactionId} با Authority {Authority} ایجاد شد",
            transaction.Id.Value, gatewayData.Authority);

        #endregion

        return new InitializePaymentResult(
            TransactionId: transaction.Id.Value,
            Authority: gatewayData.Authority,
            PaymentUrl: gatewayData.PaymentUrl);
    }
}
