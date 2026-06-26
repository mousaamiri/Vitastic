
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class CourseVideoName : ValueObject<string>
{
    public const int MaxLength = 255;

    // Supported video formats
    private static readonly string[] AllowedExtensions =
    {
        ".mp4",   // Most common
        ".mkv",   // High quality
        ".avi",   // Legacy
        ".mov",   // Apple
        ".wmv",   // Windows
        ".flv",   // Flash (legacy)
        ".webm",  // Web optimized
        ".m4v",   // iTunes
        ".mpeg",  // MPEG
        ".mpg"    // MPEG short
    };

    private CourseVideoName(string value) : base(value) { }

    /// Creates a CourseVideoName from a file name string
    public static Result<CourseVideoName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CourseVideoNameErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return CourseVideoNameErrors.TooLong(MaxLength);

        // Get extension
        var extension = Path.GetExtension(trimmed).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
            return CourseVideoNameErrors.MissingExtension;

        if (!AllowedExtensions.Contains(extension))
            return CourseVideoNameErrors.InvalidExtension(AllowedExtensions);

        // Check for invalid file name characters
        var invalidChars = Path.GetInvalidFileNameChars();
        if (trimmed.Any(c => invalidChars.Contains(c)))
            return CourseVideoNameErrors.InvalidCharacters;

        return new CourseVideoName(trimmed);
    }


    /// Gets the file extension (e.g., ".mp4")
    public string GetExtension() => Path.GetExtension(Value).ToLowerInvariant();


    /// Gets the file name without extension
    public string GetFileNameWithoutExtension() => Path.GetFileNameWithoutExtension(Value);


    /// Checks if the video is in MP4 format (recommended)
    public bool IsMp4() => string.Equals(GetExtension(), ".mp4", StringComparison.Ordinal);


    /// Checks if the video is in a web-optimized format
    public bool IsWebOptimized()
    {
        var ext = GetExtension();
        return string.Equals(ext, ".mp4", StringComparison.Ordinal) || string.Equals(ext, ".webm", StringComparison.Ordinal);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString() => Value;

    public static implicit operator string(CourseVideoName videoName) => videoName.Value;
}

public static class CourseVideoNameErrors
{
    public static Error Empty => Error.Validation(
        "CourseVideoName.Empty",
        "نام فایل ویدیو نمی‌تواند خالی باشد");

    public static Error TooLong(int max) => Error.Validation(
        "CourseVideoName.TooLong",
        $"نام فایل ویدیو نباید بیشتر از {max} کاراکتر باشد");

    public static Error MissingExtension => Error.Validation(
        "CourseVideoName.MissingExtension",
        "فایل ویدیو باید پسوند داشته باشد");

    public static Error InvalidExtension(string[] allowedExtensions) => Error.Validation(
        "CourseVideoName.InvalidExtension",
        $"فرمت فایل ویدیو پشتیبانی نمی‌شود. فقط {string.Join(", ", allowedExtensions)} مجاز است");

    public static Error InvalidCharacters => Error.Validation(
        "CourseVideoName.InvalidCharacters",
        "نام فایل شامل کاراکترهای غیرمجاز است");
}
