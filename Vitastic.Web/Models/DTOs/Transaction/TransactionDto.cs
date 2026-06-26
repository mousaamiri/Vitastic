namespace Vitastic.Web.Models.DTOs.Transaction;
public enum TransactionTypeRequest
{
    Deposit,
    Withdraw
}
public sealed record InitializePaymentRequest(
    decimal Amount,
    TransactionTypeRequest TransactionType,
    Guid? WalletId = null,
    Guid? OrderId = null,
    string? Description = null,
    string? CallbackUrl = null);

public sealed record VerifyAndCompletePaymentRequest(
    string Authority,
    string Status,
    string? CallbackRefId = null);

public record PaymentVerificationDto(
    Guid TransactionId,
    bool IsSuccess,
    int? RefId);
public sealed record InitializePaymentDto(
    Guid TransactionId,
    string Authority,
    string PaymentUrl);
