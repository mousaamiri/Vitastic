namespace Vitastic.App.Features.Payments.Dtos;

public sealed record PaymentTransactionDto
{

    public Guid Id{get; init;}
    public decimal Amount{get; init;}
    public string Currency { get; init; } = null!;
    public string Status{get; init;}=null!;
    public string? Description{get; init;}
    public DateTimeOffset TransactionDate{get; init;}
    public DateTimeOffset?  CompletedDate{get; init;}
    public string? Authority{get; init;}
    public int? RefId{get; init;}
    public Guid? WalletId{get; init;}
    public Guid? OrderId{get; init;}

    public PaymentTransactionDto() { }
}
