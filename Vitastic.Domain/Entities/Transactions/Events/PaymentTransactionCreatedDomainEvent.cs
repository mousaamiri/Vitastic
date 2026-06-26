using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Transactions.Events;

public sealed record PaymentTransactionCreatedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }
    public decimal TransactionAmount { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PaymentTransactionCreatedDomainEvent(Guid transactionId, Guid? transactionWalletId, decimal transactionAmount)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
        TransactionAmount = transactionAmount;
    }

    public static PaymentTransactionCreatedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId, Money amount)
        => new(transactionId.Value, walletId?.Value, amount.Value);
}

public sealed record PaymentTransactionFailedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PaymentTransactionFailedDomainEvent(Guid transactionId, Guid? transactionWalletId)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
    }

    public static PaymentTransactionFailedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId)
        => new(transactionId.Value, walletId?.Value);
}

public sealed record PaymentTransactionCanceledDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PaymentTransactionCanceledDomainEvent(Guid transactionId, Guid? transactionWalletId)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
    }

    public static PaymentTransactionCanceledDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId)
        => new(transactionId.Value, walletId?.Value);
}

public sealed record PaymentTransactionRevertedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }
    public decimal TransactionAmount { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PaymentTransactionRevertedDomainEvent(Guid transactionId, Guid? transactionWalletId, decimal transactionAmount)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
        TransactionAmount = transactionAmount;
    }

    public static PaymentTransactionRevertedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId, Money amount)
        => new(transactionId.Value, walletId?.Value, amount.Value);
}

public sealed record WalletCreatedDomainEvent : DomainEvent
{
    public Guid WalletId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyCode { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public WalletCreatedDomainEvent(Guid walletId, Guid userId, string currencyCode)
    {
        WalletId = walletId;
        UserId = userId;
        CurrencyCode = currencyCode;
    }

    public static WalletCreatedDomainEvent Create(WalletId walletId, UserId userId, Currency currency)
        => new(walletId.Value, userId.Value, currency.Code);
}

public sealed record TransactionCreatedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }
    public decimal TransactionAmount { get; init; }
    public string Description { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public TransactionCreatedDomainEvent(Guid transactionId, Guid? transactionWalletId, decimal transactionAmount, string description)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
        TransactionAmount = transactionAmount;
        Description = description;
    }

    public static TransactionCreatedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId, Money amount, string description)
        => new(transactionId.Value, walletId?.Value, amount.Value, description);
}

public sealed record WalletBalanceChangedDomainEvent : DomainEvent
{
    public Guid WalletId { get; init; }
    public decimal OldBalance { get; init; }
    public decimal TransactionAmount { get; init; }
    public string CurrencyCode { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public WalletBalanceChangedDomainEvent(Guid walletId, decimal oldBalance, decimal transactionAmount, string currencyCode)
    {
        WalletId = walletId;
        OldBalance = oldBalance;
        TransactionAmount = transactionAmount;
        CurrencyCode = currencyCode;
    }

    public static WalletBalanceChangedDomainEvent Create(WalletId walletId, Money oldBalance, Money transactionAmount)
        => new(walletId.Value, oldBalance.Value, transactionAmount.Value, transactionAmount.Currency.Code);
}

public sealed record TransactionFailedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public TransactionFailedDomainEvent(Guid transactionId, Guid? transactionWalletId)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
    }

    public static TransactionFailedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId)
        => new(transactionId.Value, walletId?.Value);
}

public sealed record TransactionCanceledDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public TransactionCanceledDomainEvent(Guid transactionId, Guid? transactionWalletId)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
    }

    public static TransactionCanceledDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId)
        => new(transactionId.Value, walletId?.Value);
}

public sealed record TransactionRevertedDomainEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid? TransactionWalletId { get; init; }
    public decimal TransactionAmount { get; init; }
    public string CurrencyCode { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public TransactionRevertedDomainEvent(Guid transactionId, Guid? transactionWalletId, decimal transactionAmount, string currencyCode)
    {
        TransactionId = transactionId;
        TransactionWalletId = transactionWalletId;
        TransactionAmount = transactionAmount;
        CurrencyCode = currencyCode;
    }

    public static TransactionRevertedDomainEvent Create(PaymentTransactionId transactionId, WalletId? walletId, Money amount)
        => new(transactionId.Value, walletId?.Value, amount.Value, amount.Currency.Code);
}

public sealed record PaymentTransactionCompletedDomainEvent : DomainEvent
{
    public Guid PaymentId { get; init; }
    public Guid? WalletId { get; init; }
    public Guid? OrderId { get; init; }
    public decimal AmountAmount { get; init; }
    public int RefId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PaymentTransactionCompletedDomainEvent(Guid paymentId, Guid? walletId, Guid? orderId, decimal amountAmount, int refId)
    {
        PaymentId = paymentId;
        WalletId = walletId;
        OrderId = orderId;
        AmountAmount = amountAmount;
        RefId = refId;
    }

    public static PaymentTransactionCompletedDomainEvent Create(PaymentTransactionId paymentId, WalletId? walletId, OrderId? orderId, Money amount, int refId)
        => new(paymentId.Value, walletId?.Value, orderId?.Value, amount.Value, refId);
}
