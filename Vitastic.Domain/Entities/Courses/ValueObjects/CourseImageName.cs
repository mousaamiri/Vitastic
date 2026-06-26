using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public class CourseImageName:ValueObject<string>
{
    // private const string DefaultImage = "default-course.jpg";
    private const string DefaultImage = "default.png";
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    public const int MaxLength = 255;

    private CourseImageName(string value) : base(value) { }

    public static CourseImageName Default() => new(DefaultImage);

    public static Result<CourseImageName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success(Default());

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return CourseImageNameErrors.TooLong(MaxLength);

        var extension = Path.GetExtension(trimmed).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            return CourseImageNameErrors.InvalidExtension;

        return Result.Success(new CourseImageName(trimmed));
    }

    public bool IsDefault() => string.Equals(Value, DefaultImage, StringComparison.Ordinal);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CourseImageName name) => name.Value;
}

public static class CourseImageNameErrors
{
    public static Error TooLong(int maxLenght) => Error.Validation(
        "CourseImageName.TooLong",
$"نام فایل تصویر نباید بیشتر از {maxLenght} کاراکتر باشد");

    public static Error InvalidExtension => Error.Validation(
        "CourseImageName.InvalidExtension",
        "فرمت تصویر نامعتبر است. فقط jpg, jpeg, png, webp مجاز است");
}
