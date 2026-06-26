using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.Events;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Transactions;

public class PaymentTransaction : AggregateRoot<PaymentTransactionId>
{
    // ------------------------
    // Properties
    // ------------------------
    public WalletId? WalletId { get; }
    public OrderId? OrderId { get; private set; }

    public Money Amount { get; }
    public string? Description { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public TransactionStatus Status { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTimeOffset?  CompletedDate { get; private set; }
    public DateTimeOffset?  RevertedDate { get; private set; }
    public PaymentInfo PaymentInfo { get; private set; }

    // ------------------------
    // Constructors
    // ------------------------
    protected PaymentTransaction()
    {
    } // For EF

    //TODO: We need to ensure that amount is not zero
    private PaymentTransaction(
        PaymentTransactionId id,
        Money amount,
        TransactionType transactionType,
        WalletId? walletId =null,
        string? description = null):base(id)
    {
        Amount = amount;
        Description = description;
        TransactionDate = DateTimeOffset.UtcNow;
        Status = TransactionStatus.Pending;
        WalletId = walletId;
        Type = transactionType;

        //Default payment info for pending transactions
       PaymentInfo = PaymentInfo.Create("-", 0, PaymentGateway.Wallet.Value).Value;
    }
    // ------------------------
    // Factory Method
    // ------------------------
    public static Result<PaymentTransaction> Create(Money amount,
        TransactionType transactionType
        , string? description = null,WalletId? walletId=null)
    {
        if(amount.Value <= Money.Zero().Value)
            return PaymentTransactionErrors.AmountMustBePositive;
        var transaction = new PaymentTransaction(PaymentTransactionId.New(), amount,transactionType, walletId, description);
        transaction.RaiseDomainEvent( PaymentTransactionCreatedDomainEvent.Create(
            transaction.Id,
            transaction.WalletId,
            transaction.Amount
        ));
        return transaction;
    }
    // ------------------------
    // Behaviors
    // ------------------------
    public Result AssignPaymentInfo(string authority, string provider)
    {
        if (Status != TransactionStatus.Pending)
            return PaymentTransactionErrors.CannotModifyNonPending;

        Result<PaymentInfo> paymentInfoResult = PaymentInfo.Create(authority, 0, provider);
        if (paymentInfoResult.IsFailure)
            return Result.Failure(paymentInfoResult.Error);

        PaymentInfo = paymentInfoResult.Value;

        return Result.Success();
    }
    public Result MarkCompleted(int refId)
    {
        if (Status != TransactionStatus.Pending)
            return PaymentTransactionErrors.CannotModifyNonPending;

        Result<PaymentInfo> paymentInfoResult =
            PaymentInfo.WithRefId(refId)
                .Value
                .WithPaidDate(DateTime.UtcNow);
        if (paymentInfoResult.IsFailure)
            return paymentInfoResult.Error;

        PaymentInfo = paymentInfoResult.Value;
        Status = TransactionStatus.Completed;
        CompletedDate = DateTime.UtcNow;

        RaiseDomainEvent(PaymentTransactionCompletedDomainEvent.Create(
            Id,
            WalletId,
            OrderId,
            Amount,
            refId
        ));
        return Result.Success();
    }

    public Result MarkFailed()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure(PaymentTransactionErrors.OnlyPendingCanFail);

        Status = TransactionStatus.Failed;
        CompletedDate = DateTime.UtcNow;
        PaymentInfo = PaymentInfo.WithPaidDate(DateTime.UtcNow).Value;

        RaiseDomainEvent( PaymentTransactionFailedDomainEvent.Create(Id, WalletId));

        return Result.Success();
    }
    public Result Cancel()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure(PaymentTransactionErrors.OnlyPendingCanCancel);

        Status = TransactionStatus.Canceled;
        CompletedDate = DateTime.UtcNow;
        PaymentInfo = PaymentInfo.WithPaidDate(DateTime.UtcNow).Value;

        RaiseDomainEvent( PaymentTransactionCanceledDomainEvent.Create(Id, WalletId));

        return Result.Success();
    }

    public Result Revert()
    {
        if (Status != TransactionStatus.Completed)
            return Result.Failure(PaymentTransactionErrors.OnlyCompletedCanRevert);

        Status = TransactionStatus.Reverted;
        RevertedDate = DateTime.UtcNow;

        RaiseDomainEvent( PaymentTransactionRevertedDomainEvent.Create(
            Id,
            WalletId,
            Amount
        ));

        return Result.Success();
    }
    public Result AssignToOrder(OrderId orderId)
    {
        if (orderId is null)
            return PaymentTransactionErrors.InvalidOrder;

        if (OrderId is not null)
            return PaymentTransactionErrors.AlreadyAssignedToOrder;

        OrderId = orderId;

        return Result.Success();
    }
}

public static class PaymentTransactionErrors
{
    public static Error AmountMustBePositive =>Error.Validation("PaymentTransaction.Create", "مبلغ تراکنش باید مثبت باشد.");
    public static Error CannotModifyNonPending => Error.Validation("PaymentTransaction",
        "فقط تراکنش‌های در حال انتظار قابل تغییر هستند.");
    public static Error OnlyPendingCanFail => Error.Validation("PaymentTransaction.MarkFailed",
        "فقط تراکنش‌های در حال انتظار می‌توانند به عنوان ناموفق علامت شوند.");
    public static Error OnlyPendingCanCancel => Error.Validation("PaymentTransaction.Cancel",
        "فقط تراکنش‌های در حال انتظار می‌توانند لغو شوند.");
    public static Error OnlyCompletedCanRevert => Error.Validation("PaymentTransaction.Revert",
        "فقط تراکنش‌های تکمیل شده می‌توانند برگشت داده شوند.");
    public static Error InvalidOrder => Error.Validation("PaymentTransaction.AssignToOrder", "شناسه سفارش نامعتبر است.");
    public static Error AlreadyAssignedToOrder => Error.Validation("PaymentTransaction.AssignToOrder", "این تراکنش قبلاً به یک سفارش اختصاص داده شده است.");
}
