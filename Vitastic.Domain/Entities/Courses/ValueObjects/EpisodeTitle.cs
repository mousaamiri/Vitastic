using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class EpisodeTitle : ValueObject<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 150;

    private EpisodeTitle(string value) : base(value) { }

    public static Result<EpisodeTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EpisodeTitleErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return EpisodeTitleErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return EpisodeTitleErrors.TooLong(MaxLength);

        return new EpisodeTitle(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class EpisodeTitleErrors
{
    public static Error Empty => Error.Validation(
        "EpisodeTitle.Empty",
        "عنوان قسمت نمی‌تواند خالی باشد");

    public static Error TooShort(int min) => Error.Validation(
        "EpisodeTitle.TooShort",
        $"عنوان قسمت باید حداقل {min} کاراکتر باشد");

    public static Error TooLong(int max) => Error.Validation(
        "EpisodeTitle.TooLong",
        $"عنوان قسمت نباید بیشتر از {max} کاراکتر باشد");
}
