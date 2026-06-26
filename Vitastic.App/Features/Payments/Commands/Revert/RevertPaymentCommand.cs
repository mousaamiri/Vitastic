using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Payments.Commands.Revert
{
    public sealed record RevertPaymentCommand(
        Guid TransactionId) : ICommand;
}
