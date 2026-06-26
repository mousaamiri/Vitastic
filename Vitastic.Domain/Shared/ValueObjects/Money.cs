using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;
//[Owned]
public sealed class Money : ValueObject<decimal>
{
    //------------------------
    // Properties
    //------------------------
    public Currency Currency { get; init; }
    //Ef Constructor
    private Money() : base(decimal.Zero)
    {
        Currency = Currency.IranianToman;
    }

    //------------------------
    // Constructor | Private constructor to enforce factory method usage
    //------------------------
    private Money(decimal amount, Currency currency):base(amount)
    {
        // The value in the base class is the same as the Amount.
        Value = amount;
        Currency = currency;
    }

    //-------------------------
    // ✅ Factory Method
    //-------------------------
    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return MoneyErrors.InvalidAmount(amount);
        if (string.IsNullOrWhiteSpace(currency))
            return MoneyErrors.EmptyCurrency();
        if (currency.Length != 3)
            return MoneyErrors.InvalidCurrency();
        Result<Currency> currencyResult = Currency.FromCode(currency);
        if (currencyResult.IsFailure) return MoneyErrors.InvalidCurrency();
        return Result.Success(new Money(amount, currencyResult.Value));
    }
    //Default currency
    public static string DefaultCurrencyCode => "IRT";
    public static Result<Money> CreateWithDefaultCurrency(decimal amount) => Create(amount, "IRT");
    public static Result<Money> Create(decimal amount) => Create(amount, "IRT");

    //-------------------------
    // Behaviors
    //-------------------------
    public Result<Money> Add(Money other)
    {
        if (other is null)
            return MoneyErrors.NullMoney();
        Value += other.Value;
        return !Currency.Equals(other.Currency)
            ? MoneyErrors.MismatchedCurrency()
            : Result.Success(this);
    }

    public Result<Money> Subtract(Money other)
    {
        if (other is null)
            return MoneyErrors.NullMoney();

        if (!Currency.Equals(other.Currency))
            return MoneyErrors.MismatchedCurrency();

        if (Value < other.Value)
            return MoneyErrors.InsufficientFunds();

        Value -= other.Value;
        return Result.Success(this);
    }

    public static Money Zero(Currency? currency=null)
    {
        currency ??= Currency.IranianToman;
        return new Money(0.0m, currency);
    }

    //-------------------------
    // Operators
    //-------------------------
    public static Result<Money> operator +(Money a, Money b)
    {
        if (!a.Currency.Equals(b.Currency))
            return MoneyErrors.MismatchedCurrency();

        return new Money(a.Value + b.Value, a.Currency);
    }

    public static Result<Money> operator -(Money a, Money b)
    {
        if (!a.Currency.Equals(b.Currency))
            return MoneyErrors.MismatchedCurrency();

        return new Money(a.Value - b.Value, a.Currency);
    }

    public static bool operator >(Money a, Money b) => a.Value > b.Value;
    public static bool operator <(Money a, Money b) => a.Value < b.Value;
    public static bool operator >=(Money a, Money b) => a.Value >= b.Value;
    public static bool operator <=(Money a, Money b) => a.Value <= b.Value;

    //Has value ?
    public bool HasValue => Value > 0;

    //-------------------------
    // Overrides
    //-------------------------
    public override string ToString() => $"{Value:0.00} {Currency}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }
}
