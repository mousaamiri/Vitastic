namespace Vitastic.App.Features.Payments.Dtos;

public sealed record PaymentTransactionStatusDto(
    Guid Id,
    string Status,
    bool IsPending,
    bool IsCompleted,
    bool IsFailed,
    bool IsReverted,
    DateTimeOffset TransactionDate,
    DateTimeOffset?  CompletedDate);
