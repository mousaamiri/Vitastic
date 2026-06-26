using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Orders;
public static class OrderItemErrors
{
    public static Error InvalidOrder => Error.Validation(
        "OrderItem.InvalidOrder",
        "سفارش نامعتبر است");

    public static Error InvalidCourse => Error.Validation(
        "OrderItem.InvalidCourse",
        "دوره نامعتبر است");

    public static Error InvalidCourseTitle => Error.Validation(
        "OrderItem.InvalidCourseTitle",
        "عنوان دوره نامعتبر است");

    public static Error InvalidPrice => Error.Validation(
        "OrderItem.InvalidPrice",
        "قیمت نامعتبر است");

    public static Error InvalidDiscount => Error.Validation(
        "OrderItem.InvalidDiscount",
        "مبلغ تخفیف نامعتبر است");

    public static Error DiscountMustBePositive => Error.Validation(
        "OrderItem.DiscountMustBePositive",
        "مبلغ تخفیف باید مثبت باشد");

    public static Error DiscountExceedsPrice => Error.Validation(
        "OrderItem.DiscountExceedsPrice",
        "مبلغ تخفیف نمی‌تواند بیشتر از قیمت دوره باشد");

    public static Error CurrencyMismatch => Error.Validation(
        "OrderItem.CurrencyMismatch",
        "واحد پول تخفیف با قیمت دوره مطابقت ندارد");

    public static Error AccessAlreadyGranted => Error.Conflict(
        "OrderItem.AccessAlreadyGranted",
        "دسترسی قبلاً اعطا شده است");

    public static Error AccessNotGranted => Error.Validation(
        "OrderItem.AccessNotGranted",
        "دسترسی اعطا نشده است");

    public static Error ExpiryDateMustBeInFuture => Error.Validation(
        "OrderItem.ExpiryDateMustBeInFuture",
        "تاریخ انقضا باید در آینده باشد");

    public static Error NewExpiryMustBeLater => Error.Validation(
        "OrderItem.NewExpiryMustBeLater",
        "تاریخ انقضای جدید باید بعد از تاریخ فعلی باشد");

    public static Error InvalidInstructor => Error.Validation("OrderItem.InvalidInstructor", "استاد دوره معتبر نیست.");
}
