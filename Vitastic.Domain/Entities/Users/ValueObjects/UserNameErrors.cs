using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public static class UserNameErrors
{
    public static Error TooShort(int minLength)
        =>  Error.Validation("UserName.TooShort", $"نام کاربری باید حداقل {minLength} کاراکتر باشد.");
    public static Error TooLong(int maxLength)
        =>  Error.Validation("UserName.TooLong", $"نام کاربری باید حداکثر {maxLength} کاراکتر باشد.");
    public static Error InvalidCharacters
        =>  Error.Validation("UserName.InvalidCharacters", "نام کاربری نمی‌تواند شامل کاراکترهای غیرمجاز باشد.");
    public static Error EmptyUserName()
        =>  Error.Validation("UserName.Empty", "نام کاربری نمی‌تواند خالی باشد.");
    public static Result<UserName> InvalidFormat()
        =>  Error.Validation("UserName.InvalidFormat", "نام کاربری باید فقط شامل حروف، اعداد و زیرخط باشد.");
}
