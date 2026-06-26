namespace Vitastic.Domain.Entities.Transactions.Enums;

public enum TransactionStatus
{
    Pending,
    Completed,
    Failed,
    Canceled,
    Reverted
}
public enum TransactionType
{
    Deposit,
    Withdraw
}
