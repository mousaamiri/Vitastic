using MediatR;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Messaging
{
    internal interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : INotification;
}
