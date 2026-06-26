using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Discounts;

public static class DiscountErrors
{
    public static Error InvalidPercentage => Error.Validation(
        "Discount.InvalidPercentage",
        "درصد تخفیف باید بین 0 تا 100 باشد");

    public static Error InvalidAmount => Error.Validation(
        "Discount.InvalidAmount",
        "مبلغ تخفیف باید بزرگتر از صفر باشد");

    public static Error InvalidDateRange => Error.Validation(
        "Discount.InvalidDateRange",
        "تاریخ شروع باید قبل از تاریخ پایان باشد");

    public static Error EndDateInPast => Error.Validation(
        "Discount.EndDateInPast",
        "تاریخ پایان نمی‌تواند در گذشته باشد");

    public static Error CurrencyMismatch => Error.Validation(
        "Discount.CurrencyMismatch",
        "واحد پول مطابقت ندارد");

    public static Error DiscountNotValid => Error.Validation(
        "Discount.NotValid",
        "تخفیف معتبر نیست");

    public static Error MaxMustBeBiggerThanMin => Error.Validation("Discount.MaxMustBeBiggerThanMin",
        "حداکثر مقدار باید از حداقل مقدار تخفیف بیشتر باشد. ");
    public static Error MinMustBeBiggerThanMax => Error.Validation("Discount.MinMustBeBiggerThanMax",
        "حداقل مقدار باید از حداکثر مقدار تخفیف کمتر باشد. ");

    public static Error TotalOrderAmountMustBeLessThanMaximum =>
        Error.Validation("Discount.TotalOrderAmountMustBeLessThanMaximum",
            "حداقل مبلغ خرید {} است. مبلغ خرید شما از این مقدار کمتر است.");
    public static Error TotalOrderAmountMustBeBiggerThanMinimum =>
        Error.Validation("Discount.TotalOrderAmountMustBeLessThanMaximum",
            "حداکثر مبلغ خرید {} است. مبلغ خرید شما، مبلغ خرید شما از این مقدار بیشتر است.");

    public static Error OrderBelowMinimum(decimal minimum) => Error.Validation(
        "Discount.OrderBelowMinimum",
        $"مبلغ سفارش باید حداقل {minimum:N0} باشد");

    public static Error CannotBeApplied => Error.Validation(
        "Discount.CannotBeApplied",
        "تخفیف قابل اعمال نیست");

    public static Error AlreadyUsedByUser => Error.Conflict(
        "Discount.AlreadyUsedByUser",
        "شما قبلاً از این تخفیف استفاده کرده‌اید");

    public static Error UsageLimitReached => Error.Validation(
        "Discount.UsageLimitReached",
        "ظرفیت استفاده از این تخفیف تکمیل شده است");

    public static Error InvalidMinimumAmount
        => Error.Validation(
            "Discount.InvalidMinimumAmount",
            "حداقل مبلغ تخفیف نمیتواند null باشد.");

    public static Error MinimumAmountMustBePositive =>
        Error.Validation(
            "Discount.MinimumAmountMustBePositive",
            "حداقل مبلغ تخفیف باید بزرگتر از صفر باشد");

    public static Error MaxDiscountOnlyForPercentage =>
        Error.Validation(
            "Discount.MaxDiscountOnlyForPercentage",
            "حداکثر مبلغ تخفیف فقط برای تخفیف‌های درصدی قابل تنظیم است");

    public static readonly Error InvalidMaximumAmount =
        Error.Validation(
            "Discount.InvalidMaximumAmount",
            "حداکثر مبلغ تخفیف باید بزرگتر از صفر باشد");

    public static Error MaximumAmountMustBePositive =>
        Error.Validation(
            "Discount.MaximumAmountMustBePositive",
            "حداکثر مبلغ تخفیف باید بزرگتر از صفر باشد");

    public static readonly Error UsageLimitMustBePositive =
        Error.Validation(
            "Discount.UsageLimitMustBePositive",
            "حداکثر تعداد استفاده باید بزرگتر از صفر باشد");

    public static readonly Error UsageLimitExceeded =
        Error.Validation(
            "Discount.UsageLimitExceeded",
            "تعداد استفاده از این تخفیف به حد نصاب رسیده است");

    public static Error ScopeMismatch =>
        Error.Validation(
            "Discount.ScopeMismatch",
            "دامنه تخفیف با نوع آن مطابقت ندارد");

    public static Error InvalidCourseId =>
        Error.Validation(
            "Discount.InvalidCourseId",
            "شناسه دوره نامعتبر است");

    public static Error InvalidCategoryId =>
        Error.Validation(
            "Discount.InvalidCategoryId",
            "شناسه دسته‌بندی نامعتبر است");

    public static Error InvalidInstructorId =>
        Error.Validation(
            "Discount.InvalidInstructorId",
            "شناسه مدرس نامعتبر است");

    public static readonly Error AlreadyActive =
        Error.Validation(
            "Discount.AlreadyActive",
            "تخفیف از قبل فعال است");

    public static readonly Error AlreadyInactive =
        Error.Validation(
            "Discount.AlreadyInactive",
            "تخفیف از قبل غیرفعال است");

    public static Error CannotActivateExpired =>
        Error.Validation(
            "Discount.CannotActivateExpired",
            "نمی‌توان تخفیف منقضی شده را فعال کرد");

    public static readonly Error NewEndDateMustBeLater =
        Error.Validation(
            "Discount.NewEndDateMustBeLater",
            "تاریخ پایان جدید باید بعد از تاریخ فعلی باشد");
    public static readonly Error CourseNotFound = Error.NotFound(
        "Discount.CourseNotFound",
        "دوره مورد نظر یافت نشد");
    public static Error CategoryAlreadyAdded => Error.Validation(
        "Discount.CategoryAlreadyAdded",
        "این دسته‌بندی قبلاً به تخفیف اضافه شده است");
    public static Error CategoryNotFound => Error.NotFound(
        "Discount.CategoryNotFound",
        "دسته‌بندی مورد نظر یافت نشد");
     public static Error InstructorAlreadyAdded => Error.Validation(
        "Discount.InstructorAlreadyAdded",
        "این مدرس قبلاً به تخفیف اضافه شده است");
    public static Error InstructorNotFound => Error.NotFound(
        "Discount.InstructorNotFound",
        "مدرس مورد نظر یافت نشد");
    public static Error CourseAlreadyAdded => Error.Validation(
        "Discount.CourseAlreadyAdded",
        "این دوره قبلاً به تخفیف اضافه شده است");

    public static readonly Error NotPercentageDiscount = Error.Validation(
        "Discount.NotPercentageDiscount",
        "این تخفیف از نوع درصدی نیست");

    public static readonly Error NotFixedAmountDiscount = Error.Validation(
        "Discount.NotFixedAmountDiscount",
        "این تخفیف از نوع مبلغی نیست");

    public static readonly Error CannotChangeUsedDiscount = Error.Validation(
        "Discount.CannotChangeUsedDiscount",
        "نمی‌توان مقدار تخفیفی که قبلاً استفاده شده را تغییر داد");

    public static readonly Error UsageLimitCannotBeLessThanUsedCount = Error.Validation(
        "Discount.UsageLimitCannotBeLessThanUsedCount",
        "محدودیت استفاده نمی‌تواند کمتر از تعداد استفاده‌های فعلی باشد");

    public static readonly Error InvalidFixedAmount = Error.Validation(
        "Discount.InvalidFixedAmount",
        "مقدار تخفیف نامعتبر است");

    public static readonly Error FixedAmountMustBePositive = Error.Validation(
        "Discount.FixedAmountMustBePositive",
        "مقدار تخفیف باید بیشتر از صفر باشد");

}
