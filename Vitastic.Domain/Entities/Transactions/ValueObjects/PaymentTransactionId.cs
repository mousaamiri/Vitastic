using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Transactions.ValueObjects;

public sealed class PaymentTransactionId:GuidBasedId<PaymentTransactionId>
{
    //Constructor
    private PaymentTransactionId(Guid value) : base(value) { }
    //Override
    protected override PaymentTransactionId Create(Guid value) => new(value);
    public static PaymentTransactionId New() => new(Guid.NewGuid());
    public static Result<PaymentTransactionId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new PaymentTransactionId(guid), BaseIdErrors.Empty);
    public static Result<PaymentTransactionId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new PaymentTransactionId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

