using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Categories;

public static class CategoryErrors
{
    public static Error InvalidName => Error.Validation(
        "Category.InvalidName",
        "نام دسته‌بندی نامعتبر است");

    public static Error InvalidSlug => Error.Validation(
        "Category.InvalidSlug",
        "نامک دسته‌بندی نامعتبر است");

    public static Error InvalidDisplayOrder => Error.Validation(
        "Category.InvalidDisplayOrder",
        "ترتیب نمایش باید بزرگتر از صفر باشد");

    public static Error CannotBeOwnParent => Error.Validation(
        "Category.CannotBeOwnParent",
        "دسته‌بندی نمی‌تواند والد خودش باشد");

    public static Error AlreadyActive => Error.Conflict(
        "Category.AlreadyActive",
        "دسته‌بندی قبلاً فعال است");

    public static Error AlreadyInactive => Error.Conflict(
        "Category.AlreadyInactive",
        "دسته‌بندی قبلاً غیرفعال است");

    public static Error NotFound => Error.NotFound(
        "Category.NotFound",
        "دسته‌بندی یافت نشد");
}
