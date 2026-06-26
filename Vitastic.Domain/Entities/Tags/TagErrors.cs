using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Tags;

public static class TagErrors
{
    public static Error InvalidName => Error.Validation(
        "Tag.InvalidName",
        "نام برچسب نامعتبر است");

    public static Error AlreadyActive => Error.Conflict(
        "Tag.AlreadyActive",
        "برچسب قبلاً فعال است");

    public static Error AlreadyInactive => Error.Conflict(
        "Tag.AlreadyInactive",
        "برچسب قبلاً غیرفعال است");

    public static Error NotFound => Error.NotFound(
        "Tag.NotFound",
        "برچسب یافت نشد");
}
