namespace Vitastic.App.Features.Payments.Commands.Verify
{
    public record PaymentVerificationResult(
        Guid TransactionId,
        bool IsSuccess,
        int? RefId);
}
