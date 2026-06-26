namespace Vitastic.App.Features.Payments.Dtos
{
    public sealed record InitializePaymentResult(
        Guid TransactionId,
        string Authority,
        string PaymentUrl);
}
