using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class Title : ValueObject<string>
{
    //Private fields
    public const int MaxLength = 200;
    public const int MinLength = 3;
    // Title validation rules:
    // - Must start with a letter (Persian or English)
    // - Can contain letters, numbers, spaces, and punctuation (without consecutive punctuation or spaces)
    // - No leading or trailing spaces
    // - No special characters other than basic punctuation (.,!?-…)
    // - No consecutive punctuation or spaces

    public static readonly Regex TitleRegex = new Regex(
        @"^[آ-یA-Za-z](?:(?![\p{P}\s]{2})[\p{L}\p{N}\s\p{P}\u200C…])*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    //Constructor
    private Title(string value) : base(value) { }

    //Factory Method
    public static Result<Title> Create(string value)
    {
        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return TitleErrors.TitleEmpty;
        return new Title(trimmed);
        if (trimmed.Length > MaxLength)
            return TitleErrors.TitleTooLong(MaxLength);
        if (trimmed.Length < MinLength)
            return TitleErrors.TitleTooShort(MinLength);

        if (!TitleRegex.IsMatch(trimmed))
            return TitleErrors.TitleInvalidCharacters;
        return new Title(trimmed);
    }
    //Equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    //Implicit conversion
    public static implicit operator string(Title title) => title.Value;
}

public static class TitleErrors
{
    public static Error TitleEmpty => Error.Validation(
        "Title.Empty",
        "عنوان نمی‌تواند خالی باشد.");
    public static Error TitleTooLong(int maxLength) => Error.Validation(
        "Title.TooLong",
        $"عنوان نمی‌تواند بیشتر از {maxLength} کاراکتر باشد.");
    public static Error TitleTooShort(int minLength) => Error.Validation(
        "Title.TooShort",
        $"عنوان نمی‌تواند کمتر از {minLength} کاراکتر باشد.");
    public static Error TitleInvalidCharacters => Error.Validation(
        "Title.InvalidCharacters",
        "عنوان باید با حرف شروع شود و فقط شامل حروف، اعداد، فاصله و علائم نگارشی (بدون تکرار) باشد.");
}
