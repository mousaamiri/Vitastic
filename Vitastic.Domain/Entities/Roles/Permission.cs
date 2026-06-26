using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Roles;

public class Permission : FullEntity<PermissionId>
{
    public string Code { get; private set; }
    public string Description { get; private set; }

    public const int MaxCodeLength = 100;
    public const int MinCodeLength = 3;
    public const int MaxDescriptionLength = 500;
    public const int MinDescriptionLength = 5;
    //Constructor
    private Permission(PermissionId id, string code, string description):base(id)
    {
        Code = code;
        Description = description;
    }
    //---------------------------------
    // Behaviors
    //---------------------------------
    public static Result<Permission> Create(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Permission>(PermissionErrors.CodeEmpty);

        if(code.Length > MaxCodeLength)
            return PermissionErrors.CodeTooLong(MaxCodeLength);
        if (code.Length< MinCodeLength)
            return PermissionErrors.CodeTooLong(MinCodeLength);
        //Code just can be letters, numbers and underscores and points, and must start with a letter
        var regx = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z][a-zA-Z0-9_.]*$");
        if (!regx.IsMatch(code))
            return PermissionErrors.InvalidFormat;
        if (string.IsNullOrWhiteSpace(description))
            return PermissionErrors.DescriptionEmpty;

        if(description.Length > MaxDescriptionLength)
            return PermissionErrors.DescriptionTooLong(MaxDescriptionLength);
        if (description.Length < MinDescriptionLength)
            return PermissionErrors.DescriptionTooLong(MinDescriptionLength);

        return Result.Success(new Permission(PermissionId.New(),code.ToUpper().Trim(), description.Trim()));
    }

    public Result UpdateCode(string code)
    {
        Code = code;
        MarkAsModified();
        return Result.Success();
    }

    public Result UpdateDescription(string description)
    {
        Description = description;
        MarkAsModified();
        return Result.Success();
    }
    //To string
    public override string ToString() => Code;
}

public static class PermissionErrors
{
    public static Error CodeEmpty => Error.Validation("Permission.CodeEmpty",
        "کد دسترسی نمی‌تواند خالی باشد.");

    public static Error InvalidFormat
    => Error.Validation("Permission.InvalidFormat",
        "کد دسترسی باید با یک حرف شروع شود و فقط می‌تواند شامل حروف، اعداد، زیرخط و نقطه باشد.");
    public static Error CodeTooLong(int maxLength) => Error.Validation("Permission.CodeTooLong",
        $"کد دسترسی باید حداکثر {maxLength} کاراکتر باشد.");
    public static Error DescriptionEmpty => Error.Validation("Permission.DescriptionEmpty",
        "توضیحات دسترسی نمی‌تواند خالی باشد.");
    public static Error DescriptionTooLong(int maxLength) => Error.Validation("Permission.DescriptionTooLong",
        $"توضیحات دسترسی باید حداکثر {maxLength} کاراکتر باشد.");

}
