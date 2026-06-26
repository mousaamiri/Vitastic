namespace Vitastic.Api.Features.Wallets.Responses;
public sealed record WalletBalanceResponse(
    Guid WalletId,
    decimal Balance,
    string Currency,
    bool CanWithdraw);

public sealed record WalletResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string UserFullName { get; set; }=string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
public sealed record WalletTransactionResponse
{
    // Transaction identity
    public Guid Id { get; init; }

    // Amount info
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;

    // Status & Type
    public string Status { get; init; } = string.Empty;   // Pending / Completed / Failed / Canceled / Reverted
    public string Type { get; init; } = string.Empty;     // Deposit / Withdraw / ...

    // Description
    public string? Description { get; init; }

    // Dates
    public DateTimeOffset TransactionDate { get; init; }
    public DateTimeOffset? CompletedDate { get; init; }
    public DateTimeOffset? RevertedDate { get; init; }

    // Payment info
    public string Authority { get; init; } = string.Empty;
    public int RefId { get; init; }
    public string Gateway { get; init; } = string.Empty;  // PaymentGateway.Value
    public DateTimeOffset? PaidDate { get; init; }
}
