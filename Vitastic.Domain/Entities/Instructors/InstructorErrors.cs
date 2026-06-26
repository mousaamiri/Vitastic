using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Instructors;

public static class InstructorErrors
{
    public static Error InvalidUser => Error.Validation(
        "Instructor.InvalidUser",
        "کاربر ارائه شده برای ایجاد مدرس معتبر نیست");

    public static Error SkillNotFound => Error.Validation(
        "Instructor.SkillNotFound",
        "مهارت مورد نظر یافت نشد");

    public static Error InactiveUser => Error.Validation(
        "Instructor.InactiveUser",
        "کاربر ارائه شده برای ایجاد مدرس غیرفعال است");

    public static Error InvalidFullName => Error.Validation(
        "Instructor.InvalidFullName",
        "نام کامل مدرس نامعتبر است");

    public static Error InvalidEmail => Error.Validation(
        "Instructor.InvalidEmail",
        "ایمیل مدرس نامعتبر است");

    public static Error UserIsNotAvailable => Error.Validation(
        "Instructor.UserIsNotAvailable",
        "کاربر ارائه شده برای ایجاد مدرس در دسترس نیست");

    public static Error AvatarNameIsInvalid => Error.Validation(
        "Instructor.AvatarNameInvalid",
        "نام آواتار مدرس نامعتبر است");

    public static Error ForMakeInstructorUserFirstNameMustBeProvided => Error.Validation(
        "Instructor.UserFirstNameRequired",
        "برای ایجاد مدرس، نام کاربر باید ارائه شود");

    public static Error ForMakeInstructorUserLastNameMustBeProvided => Error.Validation(
        "Instructor.UserLastNameRequired",
        "برای ایجاد مدرس، نام خانوادگی کاربر باید ارائه شود");

    public static Error ForMakeInstructorUserMustHaveAvatar => Error.Validation(
        "Instructor.UserAvatarRequired",
        "برای ایجاد مدرس، کاربر باید آواتار داشته باشد");

    // State Transition Errors
    public static Error InstructorAlreadyActive => Error.Validation(
        "Instructor.AlreadyActive",
        "مدرس قبلاً فعال است");

    public static Error CannotActivateRejectedInstructor => Error.Validation(
        "Instructor.CannotActivateRejected",
        "نمی‌توان مدرس رد شده را فعال کرد");

    public static Error CannotActivateSuspendedInstructor => Error.Validation(
        "Instructor.CannotActivateSuspended",
        "نمی‌توان مدرس معلق شده را فعال کرد");

    public static Error InstructorIsNotActive => Error.Validation(
        "Instructor.NotActive",
        "مدرس فعال نیست");

    public static Error InstructorAlreadySuspended => Error.Validation(
        "Instructor.AlreadySuspended",
        "مدرس قبلاً معلق شده است");

    public static Error CannotSuspendRejectedInstructor => Error.Validation(
        "Instructor.CannotSuspendRejected",
        "نمی‌توان مدرس رد شده را معلق کرد");

    public static Error InvalidStateForApprovalSubmission => Error.Validation(
        "Instructor.InvalidStateForApprovalSubmission",
        "وضعیت مدرس برای ارائه برای تایید معتبر نیست");

    public static Error RejectionReasonRequired => Error.Validation(
        "Instructor.RejectionReasonRequired",
        "دلیل رد کردن الزامی است");

    public static Error InstructorAlreadyRejected => Error.Validation(
        "Instructor.AlreadyRejected",
        "مدرس قبلاً رد شده است");
}
