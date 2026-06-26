using Vitastic.Api.Wrapper;

namespace Vitastic.Api.Features.Payments.Requests;
 public sealed record AssignPaymentInfoRequest(
        Guid TransactionId,
        string Authority,
        string Provider) ;
 public enum TransactionTypeRequest
 {
     Deposit,
     Withdraw
 }
public sealed record CreatePaymentTransactionRequest(
        decimal Amount,
        TransactionTypeRequest TransactionType,
        Guid? WalletId = null,
        string? Description = null) ;
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
