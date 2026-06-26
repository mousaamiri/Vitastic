using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class CourseThumbnailName : ValueObject<string>
{
    public const int MaxLength = 255;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private const string DefaultThumbnail = "default.png";
    // private const string DefaultThumbnail = "default-course-thumb.jpg";

    private CourseThumbnailName(string value) : base(value) { }


    /// Creates a CourseThumbnailName from a filename
    public static Result<CourseThumbnailName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CourseThumbnailNameErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return CourseThumbnailNameErrors.TooLong(MaxLength);

        var extension = Path.GetExtension(trimmed).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
            return CourseThumbnailNameErrors.MissingExtension;

        if (!AllowedExtensions.Contains(extension))
            return CourseThumbnailNameErrors.InvalidExtension;

        // Check for invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        if (trimmed.Any(c => invalidChars.Contains(c)))
            return CourseThumbnailNameErrors.InvalidCharacters;

        return new CourseThumbnailName(trimmed);
    }


    /// Returns the default thumbnail filename
    public static CourseThumbnailName Default() => new(DefaultThumbnail);


    /// Creates thumbnail name from course image name
    /// Example: "course-123.jpg" -> "course-123-thumb.jpg"
    public static Result<CourseThumbnailName> FromImageName(string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
            return CourseThumbnailNameErrors.Empty;

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
        var extension = Path.GetExtension(imageName);

        var thumbnailName = $"{nameWithoutExtension}-thumb{extension}";

        return Create(thumbnailName);
    }


    /// Gets the file extension (lowercase)
    public string GetExtension() => Path.GetExtension(Value).ToLowerInvariant();


    /// Gets the filename without extension
    public string GetFileNameWithoutExtension() => Path.GetFileNameWithoutExtension(Value);


    /// Checks if this is the default thumbnail
    public bool IsDefault() => Value.Equals(DefaultThumbnail, StringComparison.OrdinalIgnoreCase);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString() => Value;

    public static implicit operator string(CourseThumbnailName thumbnailName) => thumbnailName.Value;
}

public static class CourseThumbnailNameErrors
{
    public static Error Empty => Error.Validation(
        "CourseThumbnailName.Empty",
        "نام فایل تصویر بندانگشتی نمی‌تواند خالی باشد");

    public static Error TooLong(int max) => Error.Validation(
        "CourseThumbnailName.TooLong",
        $"نام فایل تصویر بندانگشتی نباید بیشتر از {max} کاراکتر باشد");

    public static Error MissingExtension => Error.Validation(
        "CourseThumbnailName.MissingExtension",
        "فایل تصویر بندانگشتی باید پسوند داشته باشد");

    public static Error InvalidExtension => Error.Validation(
        "CourseThumbnailName.InvalidExtension",
        "فرمت فایل تصویر بندانگشتی پشتیبانی نمی‌شود. فقط jpg, jpeg, png, webp, gif مجاز است");

    public static Error InvalidCharacters => Error.Validation(
        "CourseThumbnailName.InvalidCharacters",
        "نام فایل شامل کاراکترهای غیرمجاز است");
}
