using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class SectionTitle : ValueObject<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 150;

    private SectionTitle(string value) : base(value) { }

    public static Result<SectionTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return SectionTitleErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return SectionTitleErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return SectionTitleErrors.TooLong(MaxLength);

        return new SectionTitle(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class SectionTitleErrors
{
    public static Error Empty => Error.Validation(
        "SectionTitle.Empty",
        "عنوان بخش نمی‌تواند خالی باشد");

    public static Error TooShort(int min) => Error.Validation(
        "SectionTitle.TooShort",
        $"عنوان بخش باید حداقل {min} کاراکتر باشد");

    public static Error TooLong(int max) => Error.Validation(
        "SectionTitle.TooLong",
        $"عنوان بخش نباید بیشتر از {max} کاراکتر باشد");
}
