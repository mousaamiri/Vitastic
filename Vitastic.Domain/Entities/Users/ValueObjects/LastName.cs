using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class LastName:ValueObject<string>
{
    //Private
    public const int MinLength = 3;
    public const int MaxLength = 50;

    //Constructor
    private LastName(string value) : base(value) { }

    //Factory method
    public static Result<LastName> Create(string value)
    {
        if (string.IsNullOrEmpty(value.Trim()))
            return LastNameErrors.EmptyLastName();
        if (value.Any(char.IsDigit))
            return LastNameErrors.LastNameContainsDigits();
        if (value.Any(char.IsPunctuation))
            return LastNameErrors.LastNameContainsPunctuation();
        if (value.Any(char.IsSymbol))
            return LastNameErrors.LastNameContainsSymbols();
        if (value.Length < MinLength)
            return LastNameErrors.LastNameTooShort(MinLength);
        if (value.Length > MaxLength)
            return LastNameErrors.LastNameTooLong(MaxLength);
        return Result.Success(new LastName(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public static class LastNameErrors
{
    public static Error EmptyLastName() =>
         Error.Validation("LastName.Empty", "نام خانوادگی نمی‌تواند خالی باشد.");

    public static Error LastNameContainsDigits() =>  Error.Validation("LastName.ContainsDigits",
        "نام خانوادگی نمی‌تواند شامل اعداد باشد.");

    public static Error LastNameContainsPunctuation() =>  Error.Validation("LastName.ContainsPunctuation",
        "نام خانوادگی نمی‌تواند شامل علائم نگارشی باشد.");

    public static Error LastNameContainsSymbols() =>  Error.Validation("LastName.ContainsSymbols",
        "نام خانوادگی نمی‌تواند شامل نمادها باشد.");

    public static Error LastNameTooLong(int maxLength) =>  Error.Validation("LastName.TooLong",
        $"نام خانوادگی نمی‌تواند بیشتر از {maxLength} کاراکتر باشد.");

     public static Error LastNameTooShort(int minLength) =>  Error.Validation("LastName.TooShort",
        $"نام خانوادگی نمی‌تواند کمتر از {minLength} کاراکتر باشد.");
}
