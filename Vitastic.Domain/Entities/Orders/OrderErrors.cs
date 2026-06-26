using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Orders;

public static class OrderErrors
{
    public static Error InvalidUser => Error.Validation("Order.InvalidUser", "کاربر نامعتبر است");
    public static Error CanOnlyModifyPending => Error.Validation("Order.CanOnlyModifyPending", "فقط سفارشات در انتظار قابل ویرایش هستند");
    public static Error InvalidCourse => Error.Validation("Order.InvalidCourse", "دوره نامعتبر است");
    public static Error CourseAlreadyInOrder => Error.Conflict("Order.CourseAlreadyInOrder", "این دوره قبلاً به سفارش اضافه شده است");
    public static Error CurrencyMismatch => Error.Validation("Order.CurrencyMismatch", "واحد پول با سفارش مطابقت ندارد");
    public static Error ItemNotFound => Error.NotFound("Order.ItemNotFound", "آیتم در سفارش یافت نشد");
    public static Error InvalidDiscount => Error.Validation("Order.InvalidDiscount", "تخفیف نامعتبر است");
    public static Error DiscountNotValid => Error.Validation("Order.DiscountNotValid", "تخفیف معتبر نیست");
    public static Error DiscountNotApplicableToUser => Error.Validation("Order.DiscountNotApplicableToUser", "تخفیف برای این کاربر قابل اعمال نیست");
    public static Error DiscountAmountZero => Error.Validation("Order.DiscountAmountZero", "مبلغ تخفیف صفر است");
    public static Error InvalidPhoneNumber => Error.Validation("Order.InvalidPhoneNumber", "شماره تلفن نامعتبر است");
    public static Error InvalidBillingAddress => Error.Validation("Order.InvalidBillingAddress", "آدرس صورتحساب نامعتبر است");
    public static Error CannotProcessPayment => Error.Validation("Order.CannotProcessPayment", "پرداخت در وضعیت فعلی سفارش امکان‌پذیر نیست");
    public static Error InvalidTransaction => Error.Validation("Order.InvalidTransaction", "تراکنش نامعتبر است");
    public static Error InvalidGateway => Error.Validation("Order.InvalidGateway", "درگاه پرداخت نامعتبر است");
    public static Error EmptyOrder => Error.Validation("Order.EmptyOrder", "سفارش خالی است");
    public static Error OnlyProcessingCanComplete => Error.Validation("Order.OnlyProcessingCanComplete", "فقط سفارشات در حال پردازش قابل تکمیل هستند");
    public static Error CannotCompleteWithoutPayment => Error.Validation("Order.CannotCompleteWithoutPayment", "سفارش بدون پرداخت قابل تکمیل نیست");
    public static Error CannotCancel => Error.Validation("Order.CannotCancel", "سفارش در وضعیت فعلی قابل لغو نیست");
    public static Error OnlyCompletedCanRefund => Error.Validation("Order.OnlyCompletedCanRefund", "فقط سفارشات تکمیل شده قابل بازپرداخت هستند");
    public static Error InvalidPaymentMethod =>Error.Validation("Order.InvalidPaymentMethod", "لطفا شیوه پرداخت را مشخص کنید.");
    public static Error InvalidUserName =>Error.Validation("Order.InvalidUserName", "نام کاربری معتبر نیست.");
    public static Error InvalidUserEmail =>Error.Validation("Order.InvalidUserEmail", "ایمیل معتبر نیست.");
    public static Error InvalidDiscountCode =>Error.Validation("Order.InvalidDiscountCode", "کد تخفیف معتبر نیست.");
    public static readonly Error StatusAlreadySet = Error.Validation(
        "Order.StatusAlreadySet",
        "وضعیت سفارش قبلاً تنظیم شده است.");

    public static readonly Error InvalidStatusTransition = Error.Validation(
        "Order.InvalidStatusTransition",
        "تغییر وضعیت سفارش مجاز نیست.");

    public static readonly Error CustomerNoteTooLong =
        Error.Validation("Order.CustomerNoteTooLong",
            "یادداشت مشتری نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.");

    public static readonly Error AdminNoteTooLong =
        Error.Validation("Order.AdminNoteTooLong",
            "یادداشت مدیر نمی‌تواند بیشتر از ۱۰۰۰ کاراکتر باشد.");

    public static readonly Error NoteContentEmpty =
        Error.Validation("Order.NoteContentEmpty",
            "محتوای یادداشت نمی‌تواند خالی باشد.");

    public static readonly Error NoteTooLong =
        Error.Validation("Order.NoteTooLong",
            "محتوای یادداشت بیش از حد مجاز است.");

    public static readonly Error MaxNotesReached =
        Error.Validation("Order.MaxNotesReached",
            "حداکثر تعداد یادداشت‌ها (۵۰ عدد) به سفارش اضافه شده است.");

    public static readonly Error InvalidNoteId =
        Error.Validation("Order.InvalidNoteId",
            "شناسه یادداشت نامعتبر است.");

    public static readonly Error NoteNotFound =
        Error.NotFound("Order.NoteNotFound",
            "یادداشت مورد نظر یافت نشد.");

    public static readonly Error CannotRemoveCustomerNote =
        Error.Forbidden("Order.CannotRemoveCustomerNote",
            "یادداشت مشتری قابل حذف نیست.");

    public static readonly Error InvalidTaxAmount =
        Error.Validation("Order.InvalidTaxAmount", "مبلغ مالیات نامعتبر است.");

    public static readonly Error TaxAmountNegative =
        Error.Validation("Order.TaxAmountNegative", "مبلغ مالیات نمی‌تواند منفی باشد.");

    public static readonly Error InvalidShippingAmount =
        Error.Validation("Order.InvalidShippingAmount", "مبلغ حمل‌ونقل نامعتبر است.");

    public static readonly Error ShippingAmountNegative =
        Error.Validation("Order.ShippingAmountNegative", "مبلغ حمل‌ونقل نمی‌تواند منفی باشد.");}
