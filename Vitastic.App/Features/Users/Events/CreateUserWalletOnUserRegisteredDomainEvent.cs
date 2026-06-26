using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Commands.Create;
using Vitastic.Domain.Entities.Users.Events;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Events;

public class CreateUserWalletOnUserRegisteredDomainEvent
(ILogger<CreateUserWalletOnUserRegisteredDomainEvent> logger,IMediator mediator)
    :IEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken token)
    {
        logger.LogInformation(
            "ساخت کاربر تکمیل شد — در حال ساختن کیف پول کاربر{UserId}",
            notification.UserId);

        Result<Guid> result = await mediator.Send(
            new CreateWalletCommand(notification.UserId),
            token);

        if (result.IsFailure)
        {
            logger.LogCritical(
                "ساخت کیف پول برای کاربر {UserId} بعد از ساخته شدن کاربر شکست خورد: {Error}",
                notification.UserId, result.Error.Code);
        }
    }
}
