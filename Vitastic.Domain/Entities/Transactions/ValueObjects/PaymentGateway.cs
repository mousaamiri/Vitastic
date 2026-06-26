using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Transactions.ValueObjects;

public sealed class PaymentGateway : ValueObject<string>
{

    private PaymentGateway(string value) : base(value) { }

    public static readonly PaymentGateway Zarinpal = new("Zarinpal");
    public static readonly PaymentGateway Payping = new("Payping");
    public static readonly PaymentGateway IdPay = new("IdPay");
    public static readonly PaymentGateway Wallet = new("Wallet");

    public static Result<PaymentGateway> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("PaymentGateway.Empty", "درگاه پرداخت نامعتبر است");

        return value.Trim().ToLower() switch
        {
            "zarinpal" => Result.Success(Zarinpal),
            "payping" => Result.Success(Payping),
            "idpay" => Result.Success(IdPay),
            "wallet" => Result.Success(Wallet),
            _ => Error.Validation("PaymentGateway.Unsupported", $"درگاه {value} پشتیبانی نمی‌شود")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
