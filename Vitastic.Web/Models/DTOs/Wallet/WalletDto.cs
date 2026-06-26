namespace Vitastic.Web.Models.DTOs.Wallet;

public sealed record WalletDto(
    Guid Id,
    Guid UserId,
    string UserFullName,
    decimal Balance,
    string Currency,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record WalletBalanceDto(
    Guid WalletId,
    decimal Balance,
    string Currency,
    bool CanWithdraw);

public sealed record WalletTransactionDto
{
    // Transaction identity
    public Guid Id { get; init; }

    // Amount info
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;

    // Status & Type
    public string Status { get; init; } = string.Empty; // Pending / Completed / Failed / Canceled / Reverted
    public string Type { get; init; } = string.Empty; // Deposit / Withdraw / ...

    // Description
    public string? Description { get; init; }

    // Dates
    public DateTimeOffset TransactionDate { get; init; }
    public DateTimeOffset? CompletedDate { get; init; }
    public DateTimeOffset? RevertedDate { get; init; }

    // Payment info
    public string Authority { get; init; } = string.Empty;
    public int RefId { get; init; }
    public string Gateway { get; init; } = string.Empty; // PaymentGateway.Value
    public DateTimeOffset? PaidDate { get; init; }
}

public sealed record AddFundsRequest
{
    public decimal Amount { get; set; }
    public string? CallbackUrl{get; set;}
    public string? Description { get; set; } = "شارژ کیف پول توسط کاربر";
}

public sealed record WithdrawFundsRequest(
    decimal Amount,
    string? Description = null);
