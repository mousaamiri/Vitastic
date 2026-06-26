using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class Currency:ValueObject<string>
{
    //Properties
    public string Code { get; }
    public string Name { get; }
    public string Symbol { get; }
    //Fields
    public const int CodeLength = 3; // ISO 4217 standard
    /// <summary>
    /// Regex pattern for currency code validation (English uppercase only)
    /// </summary>
    public static readonly string CodePattern = @"^[A-Z]+$";
    //Constructor

    private Currency(string code, string name, string symbol):base($"{code}:{symbol}")
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }
    //Factory method
    public static Result<Currency> Create(string code, string name, string symbol)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Currency>(CurrencyErrors.CodeEmpty);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Currency>(CurrencyErrors.NameEmpty);

        if (string.IsNullOrWhiteSpace(symbol))
            return Result.Failure<Currency>(CurrencyErrors.SymbolEmpty);

        var trimmedCode = code.Trim().ToUpperInvariant();

        if (trimmedCode.Length != CodeLength) // ISO 4217 standard
            return Result.Failure<Currency>(CurrencyErrors.InvalidCodeFormat);
        //Just english letters are allowed in code
        var regx = new Regex(CodePattern);
        if (!regx.IsMatch(trimmedCode))
            return CurrencyErrors.JustEnglishLettersInCode;
        return Result.Success(new Currency(
            trimmedCode,
            name.Trim(),
            symbol.Trim()
        ));
    }
    // pre defined currencies
    public static readonly Currency IranianRial = new("IRR", "Iranian Rial", "﷼");
    public static readonly Currency IranianToman = new("IRT", "Iranian Toman", "تومان");
    public static readonly Currency UsDollar = new("USD", "US Dollar", "$");
    public static readonly Currency Euro = new("EUR", "Euro", "€");
    public static readonly Currency BritishPound = new("GBP", "British Pound", "£");
    // Helper method for get Code
    public static Result<Currency> FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Currency>(CurrencyErrors.CodeEmpty);

        var normalized = code.Trim().ToUpperInvariant();

        return normalized switch
        {
            "IRR" => Result.Success(IranianRial),
            "IRT" => Result.Success(IranianToman),
            "USD" => Result.Success(UsDollar),
            "EUR" => Result.Success(Euro),
            "GBP" => Result.Success(BritishPound),
            _ => Result.Failure<Currency>(CurrencyErrors.UnsupportedCurrency(code))
        };
    }
    //Toman to real conversion
    public Currency ToRial()
    {
        if (string.Equals(Code, "IRT", StringComparison.Ordinal))
            return IranianRial;
        return this;
    }
    //comparison
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
    //string
    public override string ToString() => $"{Name} ({Code})";
    //convert
    public static implicit operator string(Currency currency) => currency.Code;
}
public static class CurrencyErrors
{
    public static readonly Error CodeEmpty = Error.Validation("Currency.Code.Empty", "واحد پول نمی‌تواند خالی باشد.");
    public static readonly Error NameEmpty = Error.Validation("Currency.Name.Empty", "نام ارز نمی‌تواند خالی باشد.");
    public static readonly Error SymbolEmpty = Error.Validation("Currency.Symbol.Empty", "نماد ارز نمی‌تواند خالی باشد.");
    public static readonly Error InvalidCodeFormat = Error.Validation("Currency.Code.InvalidFormat", "کد ارز باید 3 حرفی باشد (استاندارد ISO 4217).");
    public static Error UnsupportedCurrency(string code) =>
        Error.Validation("Currency.Unsupported", $"ارز '{code}' پشتیبانی نمی‌شود.");
    public static readonly Error JustEnglishLettersInCode = Error.Validation("Currency.Code.InvalidCharacters", "کد ارز فقط می‌تواند شامل حروف انگلیسی باشد.");

}
