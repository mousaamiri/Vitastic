using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Transactions.ValueObjects;

public sealed class PaymentInfo:ValueObject<string>
{
    //----------------
    // Properties
    //----------------
    public string Authority { get; }
    public int RefId { get; }
    public PaymentGateway Gateway { get; }
    public DateTimeOffset?  PaidDate { get; }
    public string Description { get; }
    //----------------
    // Constructor
    //----------------


    private PaymentInfo(
        string authority,
        int refId,
        PaymentGateway gateway,
        DateTimeOffset?  paidDate,
        string description):base($"{authority}:{refId},{gateway.Value},{paidDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}")
    {
        Authority = authority;
        RefId = refId;
        Gateway = gateway;
        PaidDate = paidDate;
        Description = description;
    }

    //----------------
    // Factory Method
    //----------------
    public static Result<PaymentInfo> Create(
        string authority,
        int refId,
        string gateWayCode,
        DateTimeOffset?  paidDate = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(authority.Trim()))
            return PaymentInfoErrors.AuthorityEmpty;
        if (refId < 0)
            return PaymentInfoErrors.RefIdNegative;

        Result<PaymentGateway> gateWayResult = PaymentGateway.Create(gateWayCode);
        if(gateWayResult.IsFailure)
            return gateWayResult.Error;

        if (paidDate is not null && paidDate.Value > DateTime.UtcNow)
            return PaymentInfoErrors.PaidDateInFuture;
        return Result.Success(new PaymentInfo(
            authority.Trim(),
            refId,
            gateWayResult.Value,
            paidDate,
            description?.Trim() ?? string.Empty));
    }

    //----------------
    // Withers | With-methods (immutable updates)
    //----------------
    public Result<PaymentInfo> WithAuthority(string authority) =>
        Result.Success(new PaymentInfo(authority, RefId, Gateway, PaidDate, Description));

    public Result<PaymentInfo> WithRefId(int refId)
    {
        if(refId < 0)
            return PaymentInfoErrors.RefIdNegative;
        return Result.Success(new PaymentInfo(Authority, refId, Gateway, PaidDate, Description));
    }

    public Result<PaymentInfo> WithProvider(PaymentGateway provider) =>
        Result.Success(new PaymentInfo(Authority, RefId, Gateway, PaidDate, Description));

    public Result<PaymentInfo> WithPaidDate(DateTimeOffset?  paidDate) =>
        Result.Success(new PaymentInfo(Authority, RefId, Gateway, paidDate, Description));

    public Result<PaymentInfo> WithDescription(string description) =>
        Result.Success(new PaymentInfo(Authority, RefId, Gateway, PaidDate, description?.Trim()??string.Empty));

    //----------------
    // Overrides
    //----------------
    public override string ToString()
        => $"Payment [Authority={Authority}, " +
           $"RefId={RefId}, GateWay={Gateway.Value}," +
           $" PaidDate={PaidDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}," +
           $" Description={Description}]";
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Authority;
        yield return RefId;
        yield return Gateway;
        yield return PaidDate;
        yield return Description;
    }
}

public static class PaymentInfoErrors
{
    public static Error AuthorityEmpty => Error.Validation("PaymentInfo.AuthorityEmpty", "شناسه پرداخت (Authority) نمی‌تواند خالی باشد.");
    public static Error RefIdNegative => Error.Validation("PaymentInfo.RefIdNegative", "شناسه مرجع (RefId) نمی‌تواند منفی باشد.");
    public static Error PaidDateInFuture => Error.Validation("PaymentInfo.PaidDateInFuture", "تاریخ پرداخت نمی‌تواند در آینده باشد.");
}
