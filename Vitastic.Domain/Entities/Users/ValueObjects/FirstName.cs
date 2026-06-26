using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class FirstName:ValueObject<string>
{
    //Private Fields
    public const int MinLength = 3;
    public const int MaxLength = 50;
    //Constructor
    private FirstName(string value) : base(value) { }
    //Factory method
    public static Result<FirstName> Create(string value)
    {
        if(string.IsNullOrEmpty(value.Trim()))
            return FirstNameErrors.EmptyFirstName();
        if(value.Any(char.IsDigit))
            return FirstNameErrors.FirstNameContainsDigits();
        if(value.Any(char.IsPunctuation))
            return FirstNameErrors.FirstNameContainsPunctuation();
        if(value.Any(char.IsSymbol))
            return FirstNameErrors.FirstNameContainsSymbols();
        if(value.Length<MinLength)
            return FirstNameErrors.FirstNameTooShort(MinLength);
        if(value.Length > MaxLength)
            return FirstNameErrors.FirstNameTooLong(MaxLength);
        return Result.Success(new FirstName(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
