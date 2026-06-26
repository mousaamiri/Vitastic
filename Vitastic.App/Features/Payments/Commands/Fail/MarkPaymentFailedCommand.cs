using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Payments.Commands.Fail
{
    public sealed record MarkPaymentFailedCommand(
        Guid TransactionId) : ICommand;
}
