using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class Email : ValueObject<string>
{
    //Fields
    public const int MaxLength = 254;

    public const int MinLength = 5;

    //Constructor
    private Email(string value) : base(value) { }

    //Factory Method
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return EmailErrors.EmptyOrNullEmail;
        if (email.Length < MinLength)
            return EmailErrors.EmailTooShort(MinLength);
        if (email.Length > MaxLength) return EmailErrors.EmailTooLong(MaxLength);
        // Simple email validation
        if (!MyRegex().IsMatch(email))
            return EmailErrors.InvalidEmailFormat;

        return new Email(email);
    }

    public override string ToString() => Value;

    private static Regex MyRegex() => new(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public static class EmailErrors
{
    public static Error InvalidEmailFormat => Error.Validation("Email.InvalidFormat", "ایمیل وارد شده معتبر نیست");
    public static Error EmptyOrNullEmail => Error.Validation("Email.EmptyOrNull", "ایمیل نمی تواند خالی باشد");
    public static Error EmailTooShort(int min) => Error.Validation("Email.TooShort", $"ایمیل باید حداقل {min} کاراکتر باشد");
    public static Error EmailTooLong(int max) => Error.Validation("Email.TooLong", $"ایمیل نباید بیشتر از {max} کاراکتر باشد");
}
