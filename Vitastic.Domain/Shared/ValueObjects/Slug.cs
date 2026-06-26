using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class Slug : ValueObject<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 200;
    /// <summary>
    /// Regex pattern for slug validation (lowercase letters, numbers, and hyphens)
    /// </summary>
    public static readonly string Pattern = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";
    private Slug(string value) : base(value) { }

    public static Result<Slug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return SlugErrors.Empty;

        var slug = Normalize(value);

        if (slug.Length < MinLength)
            return SlugErrors.TooShort(MinLength);

        if (slug.Length > MaxLength)
            return SlugErrors.TooLong(MaxLength);

        if (!IsValidSlug(slug))
            return SlugErrors.InvalidFormat;

        return new Slug(slug);
    }

    /// <summary>
    /// Normalizes a string into a valid slug format
    /// </summary>
    private static string Normalize(string input)
    {
        // Convert to lowercase
        var slug = input.Trim().ToLowerInvariant();

        // Replace Persian/Arabic characters with English equivalents
        slug = ReplacePersianCharacters(slug);

        // Replace spaces and underscores with hyphens
        slug = slug.Replace(" ", "-").Replace("_", "-");

        // Remove invalid characters (keep only a-z, 0-9, and -)
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Replace multiple consecutive hyphens with single hyphen
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading and trailing hyphens
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Replaces Persian/Arabic characters with their English equivalents
    /// </summary>
    private static string ReplacePersianCharacters(string input)
    {
        var persianToEnglish = new Dictionary<char, string>
        {
            {'آ', "a"}, {'ا', "a"}, {'ب', "b"}, {'پ', "p"}, {'ت', "t"},
            {'ث', "s"}, {'ج', "j"}, {'چ', "ch"}, {'ح', "h"}, {'خ', "kh"},
            {'د', "d"}, {'ذ', "z"}, {'ر', "r"}, {'ز', "z"}, {'ژ', "zh"},
            {'س', "s"}, {'ش', "sh"}, {'ص', "s"}, {'ض', "z"}, {'ط', "t"},
            {'ظ', "z"}, {'ع', "a"}, {'غ', "gh"}, {'ف', "f"}, {'ق', "gh"},
            {'ک', "k"}, {'گ', "g"}, {'ل', "l"}, {'م', "m"}, {'ن', "n"},
            {'و', "v"}, {'ه', "h"}, {'ی', "y"}, {'ي', "y"},
            {'۰', "0"}, {'۱', "1"}, {'۲', "2"}, {'۳', "3"}, {'۴', "4"},
            {'۵', "5"}, {'۶', "6"}, {'۷', "7"}, {'۸', "8"}, {'۹', "9"}
        };

        var result = input;
        foreach (var pair in persianToEnglish)
        {
            result = result.Replace(pair.Key.ToString(), pair.Value);
        }

        return result;
    }

    /// <summary>
    /// Validates slug format (only lowercase letters, numbers, and hyphens)
    /// </summary>
    private static bool IsValidSlug(string slug)
    {
        return Regex.IsMatch(slug,Pattern);
    }

    /// <summary>
    /// Generates a slug from a title
    /// </summary>
    public static Result<Slug> FromTitle(string title)
    {
        return Create(title);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.Value;
}

public static class SlugErrors
{
    public static Error Empty => Error.Validation(
        "Slug.Empty",
        "نامک (Slug) نمی‌تواند خالی باشد");

    public static Error TooShort(int min) => Error.Validation(
        "Slug.TooShort",
        $"نامک باید حداقل {min} کاراکتر باشد");

    public static Error TooLong(int max) => Error.Validation(
        "Slug.TooLong",
        $"نامک نباید بیشتر از {max} کاراکتر باشد");

    public static Error InvalidFormat => Error.Validation(
        "Slug.InvalidFormat",
        "فرمت نامک نامعتبر است. فقط حروف انگلیسی کوچک، اعداد و خط تیره مجاز است");
}
