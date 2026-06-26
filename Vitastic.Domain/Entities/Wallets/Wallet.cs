using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.Events;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Wallets;

public class Wallet : AggregateRoot<WalletId>
{
    // Properties
    public UserId UserId { get; private set; }
    public Currency Currency { get; }
    public Money Balance { get; private set; }

    //Just ids
    private HashSet<PaymentTransactionId> _transactionIds = [];
    public IReadOnlyCollection<PaymentTransactionId> TransactionIds => _transactionIds;

    protected Wallet()
    {
    } // For EF

    private Wallet(WalletId walletId, UserId userId, Currency currency) : base(walletId)
    {
        UserId = userId;
        Currency = currency;
        Balance = Money.Create(0, currency.Code).Value;
    }

    // ------------------------
    // Factory method
    // ------------------------
    public static Result<Wallet> Create(UserId userId, string? currencyCode = "IRT")
    {
        if (userId is null)
            return (WalletErrors.InvalidUser);

        Result<Currency> currencyResult = Currency.FromCode(currencyCode ?? Currency.IranianToman);
        if (currencyResult.IsFailure)
            return (currencyResult.Error);

        var wallet = new Wallet(
            WalletId.New(),
            userId,
            currencyResult.Value
        );

        wallet.RaiseDomainEvent( WalletCreatedDomainEvent.Create(
            wallet.Id,
            wallet.UserId,
            wallet.Currency
        ));

        return Result.Success(wallet);
    }

    //Create with currency object
    public static Result<Wallet> Create(UserId userId, Currency currency)
    {
        if (userId is null)
            return (WalletErrors.InvalidUser);

        if (currency is null)
            return (WalletErrors.InvalidCurrency);

        var wallet = new Wallet(
            WalletId.New(),
            userId,
            currency
        );

        wallet.RaiseDomainEvent( WalletCreatedDomainEvent.Create(
            wallet.Id,
            wallet.UserId,
            wallet.Currency
        ));

        return Result.Success(wallet);
    }

    // ------------------------
    // Transaction Creation
    // ------------------------
    public Result<PaymentTransaction> CreateWithdrawalTransaction(
        Money amount,
        string? description = null)
    {
        if (amount.Value <= 0)
            return Result.Failure<PaymentTransaction>(WalletErrors.AmountMustBePositive);

        if (!string.Equals(amount.Currency, Currency.Code, StringComparison.Ordinal))
            return WalletErrors.CurrencyMismatch(amount.Currency, Currency.Code);

        if (Balance.Value < amount.Value)
            return WalletErrors.InsufficientBalance(Balance.Value, amount.Value);

        // Negate the amount for withdrawal
        Result<Money> negativeAmountResult = Money.Create(amount.Value, Currency.Code);
        if (negativeAmountResult.IsFailure)
            return negativeAmountResult.Error;

        Result<PaymentTransaction> transactionResult = PaymentTransaction.Create(
            negativeAmountResult.Value,
            TransactionType.Withdraw,
            description ?? $"برداشت از کیف پول {Currency.Name}",
            Id
        );

        if (transactionResult.IsFailure)
            return transactionResult;

        _transactionIds.Add(transactionResult.Value.Id);

        RaiseDomainEvent( TransactionCreatedDomainEvent.Create(
            transactionResult.Value.Id,
            Id,
            amount,
            "Withdrawal"
        ));

        return transactionResult;
    }

    public Result<PaymentTransaction> CreateDepositTransaction(
        Money amount,
        string? description = null)
    {
        if (amount.Value <= 0)
            return
                WalletErrors.AmountMustBePositive;

        if (!string.Equals(amount.Currency, Currency.Code, StringComparison.Ordinal))
            return
                WalletErrors.CurrencyMismatch(amount.Currency, Currency.Code);

        Result<PaymentTransaction> transactionResult = PaymentTransaction.Create(
            amount,
            TransactionType.Deposit,
            description ?? $"واریز به کیف پول {Currency.Name}",
            Id
        );

        if (transactionResult.IsFailure)
            return transactionResult;

        _transactionIds.Add(transactionResult.Value.Id);

        RaiseDomainEvent( TransactionCreatedDomainEvent.Create(
            transactionResult.Value.Id,
            Id,
            amount,
            "Deposit"
        ));

        return transactionResult;
    }

    // ------------------------
    // Transaction Management
    // ------------------------
    public void InitTransactionIds(IEnumerable<PaymentTransactionId> transactionIds)
    {
        _transactionIds = transactionIds.ToHashSet();
    }
    public Result<PaymentTransaction> CompleteTransaction(PaymentTransaction transaction)
    {
        if (transaction is null)
            return WalletErrors.InvalidTransaction;
        if (transaction.WalletId is null)
            return WalletErrors.InvalidTransaction;
        if (!transaction.WalletId.Equals(Id))
            return WalletErrors.TransactionNotBelongsToWallet;
        if (!_transactionIds.Contains(transaction.Id))
            return WalletErrors.TransactionNotFound;
        if (transaction.Status != TransactionStatus.Completed)
            return WalletErrors.TransactionNotCompleted;

        // Check Currency
        if (!string.Equals(transaction.Amount.Currency, Currency.Code, StringComparison.Ordinal))
            return WalletErrors.CurrencyMismatch(transaction.Amount.Currency, Currency.Code);

        // Update Transaction Status
        Result<Money> newBalanceResult;
        if (transaction.Type.Equals(TransactionType.Deposit))
        {
            newBalanceResult = Balance.Add(transaction.Amount);
            if (newBalanceResult.IsFailure)
                return newBalanceResult.Error;
            Balance = newBalanceResult.Value;
            transaction.MarkCompleted(0);
        }
        else if (transaction.Type.Equals(TransactionType.Withdraw))
        {
            newBalanceResult = Balance.Subtract(transaction.Amount);
            if (newBalanceResult.IsFailure)
                return newBalanceResult.Error;
            Balance = newBalanceResult.Value;
            transaction.MarkCompleted(0);
        }

        RaiseDomainEvent( WalletBalanceChangedDomainEvent.Create(
            Id,
            Balance,
            transaction.Amount
        ));

        return transaction;
    }

    public Result FailTransaction(PaymentTransaction transaction)
    {
        if (transaction is null)
            return WalletErrors.InvalidTransaction;
        if (transaction.WalletId is null)
            return WalletErrors.InvalidTransaction;

        if (!transaction.WalletId.Equals(Id))
            return WalletErrors.TransactionNotBelongsToWallet;

        if (!_transactionIds.Contains(transaction.Id))
            return WalletErrors.TransactionNotFound;

        RaiseDomainEvent( TransactionFailedDomainEvent.Create(
            transaction.Id,
            Id
        ));

        return Result.Success();
    }

    public Result CancelTransaction(PaymentTransaction transaction)
    {
        if (transaction is null)
            return WalletErrors.InvalidTransaction;
        if (transaction.WalletId is null)
            return WalletErrors.InvalidTransaction;

        if (!transaction.WalletId.Equals(Id))
            return WalletErrors.TransactionNotBelongsToWallet;

        if (!_transactionIds.Contains(transaction.Id))
            return WalletErrors.TransactionNotFound;

        RaiseDomainEvent( TransactionCanceledDomainEvent.Create(
            transaction.Id,
            Id
        ));

        return Result.Success();
    }

    public Result RevertTransaction(PaymentTransaction transaction)
    {
        if (transaction is null)
            return WalletErrors.InvalidTransaction;


        if (transaction.WalletId ==null || !transaction.WalletId!.Equals(Id))
            return WalletErrors.TransactionNotBelongsToWallet;


        if (!_transactionIds.Contains(transaction.Id))
            return WalletErrors.TransactionNotFound;


        if (transaction.Status != TransactionStatus.Completed)
            return WalletErrors.OnlyCompletedCanRevert;

        // Check Currency
        if (!string.Equals(transaction.Amount.Currency, Currency.Code, StringComparison.Ordinal))
            return WalletErrors.CurrencyMismatch(
                transaction.Amount.Currency,
                Currency.Code);

        // Revert the transaction by subtracting the amount from the balance
        Result<Money> newBalanceResult = Balance.Subtract(transaction.Amount);
        if (newBalanceResult.IsFailure)
            return newBalanceResult.Error;
        var revertResult = transaction.Revert();
        if(revertResult.IsFailure)
            return revertResult.Error;

        Balance = newBalanceResult.Value;
        RaiseDomainEvent( TransactionRevertedDomainEvent.Create(
            transaction.Id,
            Id,
            transaction.Amount
        ));

        return Result.Success();
    }
    // ------------------------
    // Simplified Operations
    // ------------------------

    /// Fast deposit (no gateway confirmation required)
    public Result<PaymentTransaction> AddFunds(decimal amount, string? description = null)
    {
        var moneyResult = Money.Create(amount, Currency.Code);
        if (moneyResult.IsFailure)
            return moneyResult.Error;
        var transactionResult = CreateDepositTransaction(moneyResult.Value, description);
        if (transactionResult.IsFailure)
            return transactionResult;

        PaymentTransaction transaction = transactionResult.Value;

        // Autocomplete (for internal deposits)
        Result completeResult = CompleteTransaction(transaction);
        if (completeResult.IsFailure)
            return Result.Failure<PaymentTransaction>(completeResult.Error);

        return Result.Success(transaction);
    }


    /// Fast withdrawal
    public Result<PaymentTransaction> WithdrawFunds(decimal amount, string? description = null)
    {
        if (amount <= 0)
            return WalletErrors.AmountMustBePositive;
        var moneyResult = Money.Create(amount, Currency.Code);
        if (moneyResult.IsFailure) return moneyResult.Error;
        Result<PaymentTransaction> transactionResult = CreateWithdrawalTransaction(moneyResult.Value, description);
        if (transactionResult.IsFailure)
            return transactionResult;

        PaymentTransaction transaction = transactionResult.Value;

        // Autocomplete
        Result completeResult = CompleteTransaction(transaction);
        if (completeResult.IsFailure)
            return Result.Failure<PaymentTransaction>(completeResult.Error);

        return Result.Success(transaction);
    }
    // ------------------------
    // Queries
    // ------------------------

    public bool HasSufficientBalance(Money amount) =>
        string.Equals(amount.Currency, Currency.Code, StringComparison.Ordinal) && Balance.Value >= amount.Value;

    public bool CanWithdraw(Money amount) => HasSufficientBalance(amount);
    public Result<Money> GetAvailableBalance() => Result.Success(Balance);

    /// Convert balance to another currency (for display)
    public Money GetBalanceIn(Currency targetCurrency, decimal exchangeRate)
    {
        if (string.Equals(Currency.Code, targetCurrency.Code, StringComparison.Ordinal))
            return Balance;

        var convertedAmount = Balance.Value * exchangeRate;
        return Money.Create(convertedAmount, targetCurrency.Code).Value;
    }

}
