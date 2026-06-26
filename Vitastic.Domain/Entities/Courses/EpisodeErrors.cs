using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses;
public static class EpisodeErrors
{
    public static Error InvalidSectionId => Error.Validation(
        "Episode.InvalidSectionId",
        "شناسه بخش نامعتبر است");

    public static Error InvalidTitle => Error.Validation(
        "Episode.InvalidTitle",
        "عنوان قسمت نامعتبر است");

    public static Error InvalidDuration => Error.Validation(
        "Episode.InvalidDuration",
        "مدت زمان قسمت نامعتبر است");

    public static Error InvalidPrice => Error.Validation(
        "Episode.InvalidPrice",
        "قیمت قسمت نامعتبر است");

    public static Error InvalidDisplayOrder => Error.Validation(
        "Episode.InvalidDisplayOrder",
        "ترتیب نمایش نامعتبر است");

    public static Error InvalidVideoName => Error.Validation(
        "Episode.InvalidVideoName",
        "نام فایل ویدیو نامعتبر است");

    public static Error CurrencyMismatch => Error.Validation(
        "Episode.CurrencyMismatch",
        "واحد پول با دوره مطابقت ندارد");
}
