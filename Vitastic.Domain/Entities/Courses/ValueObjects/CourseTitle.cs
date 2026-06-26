using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class CourseTitle : ValueObject<string>
{
    //Private fields
    public const int MinLength = 3;
    public const int MaxLength = 200;
    // Title validation rules:
    // - Must start with a Persian (U+0600–U+06FF) or Latin letter
    // - May contain Persian/Latin letters, digits, spaces, ZWNJ (U+200C), and punctuation
    // - Allows multiple consecutive spaces but not consecutive punctuation
    private static Regex TitleRegex = new(
        @"^[\u0600-\u06FFa-zA-Z0-9]+([\s\u200C\p{P}][\u0600-\u06FFa-zA-Z0-9]+)*$",
        RegexOptions.Compiled
    );



    //Constructor
    private CourseTitle(string value) : base(value) { }
    //Factory methods
    public static Result<CourseTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CourseTitleErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return CourseTitleErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return CourseTitleErrors.TooLong(MaxLength);
        if (!TitleRegex.IsMatch(trimmed))
            return CourseTitleErrors.TitleInvalidCharacters;
        return Result.Success(new CourseTitle(trimmed));
    }
    public static CourseTitle CreateUnsafe(string value) => new CourseTitle(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CourseTitle title) => title.Value;
}

public static class CourseTitleErrors
{
    public static Error Empty => Error.Validation(
        "CourseTitle.Empty",
        "عنوان دوره نمی‌تواند خالی باشد");

    public static Error TooShort(int minLength) => Error.Validation(
        "CourseTitle.TooShort",
$"عنوان دوره باید حداقل {minLength} کاراکتر باشد");

    public static Error TooLong(int maxLenght) => Error.Validation(
        "CourseTitle.TooLong",
$"عنوان دوره نباید بیشتر از {maxLenght} کاراکتر باشد");
    public static Error TitleInvalidCharacters => Error.Validation(
        "Title.InvalidCharacters",
        "عنوان باید با حرف شروع شود و فقط شامل حروف، اعداد، فاصله و علائم نگارشی (بدون تکرار) باشد.");
}
