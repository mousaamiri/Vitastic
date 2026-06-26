using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class EpisodeVideoName : ValueObject<string>
{
    public const int MaxLength = 255;
    private static readonly string[] AllowedExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm" };

    private EpisodeVideoName(string value) : base(value) { }

    public static Result<EpisodeVideoName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EpisodeVideoNameErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return EpisodeVideoNameErrors.TooLong(MaxLength);

        var extension = Path.GetExtension(trimmed).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
            return EpisodeVideoNameErrors.MissingExtension;

        if (!AllowedExtensions.Contains(extension))
            return EpisodeVideoNameErrors.InvalidExtension;

        // Check for invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        if (trimmed.Any(c => invalidChars.Contains(c)))
            return EpisodeVideoNameErrors.InvalidCharacters;

        return new EpisodeVideoName(trimmed);
    }

    public string GetExtension() => Path.GetExtension(Value).ToLowerInvariant();

    public string GetFileNameWithoutExtension() => Path.GetFileNameWithoutExtension(Value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(EpisodeVideoName videoName) => videoName.Value;
}

public static class EpisodeVideoNameErrors
{
    public static Error Empty => Error.Validation(
        "EpisodeVideoName.Empty",
        "نام فایل ویدیو نمی‌تواند خالی باشد");

    public static Error TooLong(int max) => Error.Validation(
        "EpisodeVideoName.TooLong",
        $"نام فایل ویدیو نباید بیشتر از {max} کاراکتر باشد");

    public static Error MissingExtension => Error.Validation(
        "EpisodeVideoName.MissingExtension",
        "فایل ویدیو باید پسوند داشته باشد");

    public static Error InvalidExtension => Error.Validation(
        "EpisodeVideoName.InvalidExtension",
        "فرمت فایل ویدیو پشتیبانی نمی‌شود. فقط mp4, mkv, avi, mov, wmv, flv, webm مجاز است");

    public static Error InvalidCharacters => Error.Validation(
        "EpisodeVideoName.InvalidCharacters",
        "نام فایل شامل کاراکترهای غیرمجاز است");
}
