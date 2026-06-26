using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Payments.Commands.Cancel
{
    public sealed record CancelPaymentCommand(Guid TransactionId) : ICommand;
}
