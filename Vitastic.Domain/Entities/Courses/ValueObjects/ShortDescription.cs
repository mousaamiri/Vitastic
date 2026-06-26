using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class ShortDescription : ValueObject<string>
{
    public const int MinLength = 20;
    public const int MaxLength = 500;

    private ShortDescription(string value) : base(value) { }

    public static Result<ShortDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ShortDescriptionErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return ShortDescriptionErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return ShortDescriptionErrors.TooLong(MaxLength);

        return new ShortDescription(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public static class ShortDescriptionErrors
{
    public static Error Empty => Error.Validation(
        "ShortDescription.Empty",
        "توضیحات کوتاه نمی‌تواند خالی باشد");

    public static Error TooShort(int min) => Error.Validation(
        "ShortDescription.TooShort",
        $"توضیحات کوتاه باید حداقل {min} کاراکتر باشد");

    public static Error TooLong(int max) => Error.Validation(
        "ShortDescription.TooLong",
        $"توضیحات کوتاه نباید بیشتر از {max} کاراکتر باشد");
}
