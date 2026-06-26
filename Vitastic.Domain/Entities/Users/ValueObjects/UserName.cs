using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class UserName:ValueObject<string>
{

    //Fields
    public const int MinLength = 3;
    public const int MaxLength = 50;

    //Constructor
    private UserName(string value) : base(value) { }

    //Factory method
    public static Result<UserName> Create(string value)
    {
        if (string.IsNullOrEmpty(value.Trim()))
            return UserNameErrors.EmptyUserName();
        switch (value.Length)
        {
            case < MinLength:
                return UserNameErrors.TooShort(MinLength);
            case > MaxLength:
                return UserNameErrors.TooLong(MaxLength);
        }

        var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_]+$"); if (!regex.IsMatch(value))
            return UserNameErrors.InvalidFormat();
        return Result.Success(new UserName(value));
    }

    //Overrider ToString
    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
