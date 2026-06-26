using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class UserAvatar:ValueObject<string>
{
    // Public object
    public string FileName { get; }

    // Private fields
    private const string DefaultAvatar = "default.png";
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    public const int MaxFileNameLength = 255;

    //Constructor
    private UserAvatar(string fileName):base(fileName)
    {
        FileName = fileName;
    }

    //Default avatar
    public static UserAvatar Default() => new(DefaultAvatar);

    //Factory method
    public static Result<UserAvatar> Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Result.Success(Default());

        var trimmed = fileName.Trim();

        if (trimmed.Length > MaxFileNameLength)
            return UserAvatarErrors.FileNameTooLong(MaxFileNameLength);

        var extension = Path.GetExtension(trimmed).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            return UserAvatarErrors.InvalidExtension(AllowedExtensions);

        // Check for invalid characters in file name
        if (ContainsInvalidCharacters(trimmed))
            return UserAvatarErrors.InvalidCharacters;

        return Result.Success(new UserAvatar(trimmed));
    }
    //Check is default avatar
    public bool IsDefault() => FileName.Equals(DefaultAvatar, StringComparison.OrdinalIgnoreCase);
    //Get full path of avatar
    public string GetFullPath(string baseUrl) => $"{baseUrl.TrimEnd('/')}/{FileName}";
    //Get file extension
    public string GetExtension() => Path.GetExtension(FileName).ToLowerInvariant();
    //Check contains invalid characters
    private static bool ContainsInvalidCharacters(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return fileName.Any(c => invalidChars.Contains(c));
    }
    //Get equality components for value object comparison
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
    }
    //ToString
    public override string ToString() => FileName;
    // Implicit conversion to string
    public static implicit operator string(UserAvatar avatar) => avatar.FileName;
}

public static class UserAvatarErrors
{
    public static Result<UserAvatar> InvalidUrl(string url)
        =>  Error.Validation("UserAvatar.InvalidUrl", $"این آدرس {url} یک آدرس معتبر برای تصویر پروفایل نیست");
    public static Result<UserAvatar> FileNameTooLong(int maxLength)
        =>  Error.Validation("UserAvatar.FileNameTooLong", $"نام فایل نمی‌تواند بیشتر از {maxLength} کاراکتر باشد");
    public static Result<UserAvatar> InvalidExtension(string[] allowedExtensions)
        =>  Error.Validation("UserAvatar.InvalidExtension",
            $"فایل انتخاب شده باید یکی از فرمت‌های {string.Join(", ", allowedExtensions)} باشد");
    public static Result<UserAvatar> InvalidCharacters
        =>  Error.Validation("UserAvatar.InvalidCharacters",
            $"نام فایل نمی‌تواند حاوی کاراکترهای غیرمجاز مانند / \\ : * ? \" < > | باشد");
}
