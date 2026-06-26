using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions;

namespace Vitastic.App.Features.Payments.Commands.Verify
{
    public sealed record VerifyAndCompletePaymentCommand(
        string Authority,
        string Status,
        string? CallbackRefId = null) : ICommand<PaymentVerificationResult>;
}
