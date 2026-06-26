using MediatR;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Commands.CompleteWalletTransaction;
using Vitastic.Domain.Entities.Transactions.Events;

namespace Vitastic.App.Features.Wallets.Events;

public class UpdateWalletOnTransactionCompletedHandler
(ILogger<UpdateWalletOnTransactionCompletedHandler> logger,IMediator mediator)
    :IEventHandler<PaymentTransactionCompletedDomainEvent>
{
    public async Task Handle(PaymentTransactionCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.WalletId is not null)
        {
            logger.LogInformation(
                "پرداخت {TransactionId} تکمیل شد — در حال بروز رسانی کیف پول {WalletId}",
                notification.PaymentId, notification.WalletId);

            var result = await mediator.Send(
                new CompleteWalletTransactionCommand(notification.WalletId.Value,notification.PaymentId),
                cancellationToken);

            if (result.IsFailure)
            {
                logger.LogCritical(
                    "بروز رسانی کیف پول {WalletId} بعد از پرداخت شکست خورد: {Error}",
                    notification.WalletId, result.Error.Code);
            }
        }

    }
}
