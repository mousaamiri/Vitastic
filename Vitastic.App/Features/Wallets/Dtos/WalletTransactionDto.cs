namespace Vitastic.App.Features.Wallets.Dtos
{

    public sealed record WalletTransactionDto
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


}
