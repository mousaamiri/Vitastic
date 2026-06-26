using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public class FullName:ValueObject<string>
{
    //Private Fields
    public const int MinLength = 2;
    public const int MaxLength = 100;
    protected FullName(string value) : base(value) { }

    public static Result<FullName> Create(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName.Trim()))
            return FullNameErrors.EmptyFullName();

        if (fullName.Length < MinLength)
            return FullNameErrors.FullNameTooShort(MinLength);

        if (fullName.Length > MaxLength)
            return FullNameErrors.FullNameTooLong(MaxLength);

        foreach (char c in fullName)
        {
            if (char.IsDigit(c))
                return FullNameErrors.FullNameContainsDigits();
            if (char.IsPunctuation(c))
                return FullNameErrors.FullNameContainsPunctuation();
            if (char.IsSymbol(c))
                return FullNameErrors.FullNameContainsSymbols();
        }

        return new FullName(fullName);
    }
    public static Result<FullName> Create(string firestName, string lastName)
    {
        var fullName = $"{firestName} {lastName}";
        return Create(fullName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    //Converter
    public static implicit operator string(FullName fullName) => fullName.Value;
    public static explicit operator FullName?(string fullName) => Create(fullName).Value;
}
public static class FullNameErrors
{
    public static Error EmptyFullName() =>
         Error.Validation("FullName.Empty", "نام کامل نمیتواند خالی باشد.");
    public static Error FullNameContainsDigits() =>
         Error.Validation("FullName.ContainsDigits", "نام کامل نمیتواند شامل اعداد باشد.");
    public static Error FullNameContainsPunctuation() =>
         Error.Validation("FullName.ContainsPunctuation", "نام کامل نمیتواند شامل علائم نگارشی باشد.");
    public static Error FullNameContainsSymbols() =>
         Error.Validation("FullName.ContainsSymbols", "نام کامل نمیتواند شامل نمادها باشد.");
    public static Error FullNameTooShort(int minLength) =>
         Error.Validation("FullName.TooShort", $"نام کامل نمیتواند کمتر از {minLength} کاراکتر باشد.");
    public static Error FullNameTooLong(int maxLength) =>
         Error.Validation("FullName.TooLong", $"نام کامل نمیتواند بیشتر از {maxLength} کاراکتر باشد.");

}
