using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Commands.CompleteOrder;
using Vitastic.App.Features.Wallets.Commands.Create;
using Vitastic.Domain.Entities.Users.Events;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Events;

public class CreateUserWalletOnUserCreatedByAdminDomainEvent(
    ILogger<CreateUserWalletOnUserCreatedByAdminDomainEvent> logger,
    IMediator mediator)
    : IEventHandler<UserCreatedByAdminDomainEvent>
{
    public async Task Handle(UserCreatedByAdminDomainEvent notification, CancellationToken token)
    {
        logger.LogInformation(
            "ساخت کاربر تکمیل شد(به وسیله ادمین) — در حال ساختن کیف پول کاربر{UserId}",
            notification.UserId);

        Result<Guid> result = await mediator.Send(
            new CreateWalletCommand(notification.UserId),
            token);

        if (result.IsFailure)
        {
            logger.LogCritical(
                "ساخت کیف پول برای کاربر {UserId} (ساخته شده به وسیله ادمین) بعد از ساخته شدن کاربر شکست خورد: {Error}",
                notification.UserId, result.Error.Code);
        }
    }
}
