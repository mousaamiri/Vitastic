namespace Vitastic.Api.Features.Payments.Responses;
public sealed record InitializePaymentResult(
    Guid TransactionId,
    string Authority,
    string PaymentUrl);
public sealed record PaymentTransactionResponse(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    string? Description,
    DateTimeOffset TransactionDate,
    DateTimeOffset?  CompletedDate,
    string? Authority,
    int? RefId,
    Guid? WalletId,
    Guid? OrderId);
public sealed record PaymentTransactionStatusResponse(
    Guid Id,
    string Status,
    bool IsPending,
    bool IsCompleted,
    bool IsFailed,
    bool IsReverted,
    DateTimeOffset TransactionDate,
    DateTimeOffset?  CompletedDate);
public record PaymentVerificationResponse(
    Guid TransactionId,
    bool IsSuccess,
    int? RefId);
public sealed record InitializePaymentResponse(
    Guid TransactionId,
    string Authority,
    string PaymentUrl);
