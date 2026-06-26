using MediatR;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Commands.CompleteOrder;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Events;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using TransactionStatus = Vitastic.Domain.Entities.Transactions.Enums.TransactionStatus;

namespace Vitastic.App.Features.Payments.Events;

public class CompleteOrderOnTransactionCompletedHandler(
    ILogger<CompleteOrderOnTransactionCompletedHandler> logger,
    IMediator mediator)
    : IEventHandler<PaymentTransactionCompletedDomainEvent>
{
    public async Task Handle(
        PaymentTransactionCompletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        if (notification.OrderId is not null)
        {
            logger.LogInformation(
                "پرداخت {TransactionId} تکمیل شد — در حال تکمیل سفارش {OrderId}",
                notification.PaymentId, notification.OrderId);

            var result = await mediator.Send(
                new CompleteOrderCommand(notification.OrderId.Value, notification.PaymentId),
                cancellationToken);

            if (result.IsFailure)
            {
                logger.LogCritical(
                    "تکمیل سفارش {OrderId} بعد از پرداخت شکست خورد: {Error}",
                    notification.OrderId, result.Error.Code);
                throw new Exception(result.Error.Message);
            }
        }
    }
}
