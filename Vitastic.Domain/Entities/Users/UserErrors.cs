using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Users
{
    public static class UserErrors
    {
        public static Error UserIsActive => Error.Validation("User.IsActive", "کاربر فعال است.");
        public static Error UserIsInactive => Error.Validation("User.IsInactive", "کاربر غیرفعال است.");

        public static Error InvalidActivationCode =>
            Error.Verification("User.InvalidActivationCode", "کد فعال سازی نامعتبر است.");

        public static Error AlreadyInactive => Error.Validation("User.AlreadyInactive", "کاربر از قبل غیرفعال است.");
        public static Error AlreadyActive => Error.Validation("User.AlreadyActive", "کاربر از قبل فعال است.");
        public static Error InvalidPassword => Error.Validation("User.InvalidPassword", "رمز عبور نامعتبر است.");

        public static Error InvalidAvatar => Error.Validation("User.InvalidAvatar", "آواتار نامعتبر است.");

        //User - Role errors
        public static Error RoleIsNull => Error.Validation("User.RoleIsNull", "نقش نمی‌تواند null باشد.");
        public static Error RoleNotFound => Error.NotFound("User.RoleNotFound", "نقش مورد نظر یافت نشد.");

        public static Error UserAlreadyHasRole =>
            Error.Validation("User.AlreadyHasRole", "کاربر از قبل این نقش را دارد.");

        public static Error UserDoesNotHaveRole => Error.Validation("User.DoesNotHaveRole", "کاربر این نقش را ندارد.");

        public static Error RoleHasNotPermission =>
            Error.Validation("User.RoleHasNotPermission", "نقش باید حداقل یک دسترسی داشته باشد.");

        //User - Authentication errors
        public static Error AccountNotActivated => Error.Validation("User.AccountNotActivated", "حساب کاربری فعال نشده است.");
        public static Error NoActivationCodeAvailable=> Error.Validation("User.NoActivationCodeAvailable", "هیچ کد فعال سازی در دسترس نیست.");
        public static Error ActiveCodeIsExpired => Error.Validation("User.ActiveCodeIsExpired",
            "این کد فعالسازی منقضی شده است. لطفا مجدد درخواست کنید.");

        public static Error NoResetTokenAvailable
            =>  Error.Verification("User.NoResetTokenAvailable", "هیچ توکن بازیابی رمز عبوری در دسترسی نیست .");

        public static Error InvalidResetToken
            => Error.Verification("User.InvalidResetToken", "توکن بازیابی معتبر نیست.");

        public static Error ResetTokenExpired
            => Error.Verification("User.ResetTokenExpired", "توکن بازیابی منقضی شده است.");

        public static Error NotFound
            => Error.Verification("User.NotFound", "کاربر یافت نشد.");

        public static Error InvalidRefreshToken =>
            Error.Validation("User.InvalidRefreshToken", "رفرش توکن نامعتبر است.");
        public static Error UserNotFound =>
            Error.Validation("User.UserNotFound", "کاربر یافت نشد.");
        public static Error EmailAlreadyExists =>
            Error.Validation("User.EmailAlreadyExists", "این ایمیل از قبل ثبت شده است. ");

        public static Error UserNameAlreadyExists =>
            Error.Validation("User.UserNameAlreadyExists", "این نام کاربری از قبل موجود است. ");
    }
}
