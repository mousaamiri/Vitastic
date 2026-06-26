using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses;

public static class SectionErrors
{
    public static Error InvalidCourseId => Error.Validation(
        "Section.InvalidCourseId",
        "شناسه دوره نامعتبر است");

    public static Error InvalidTitle => Error.Validation(
        "Section.InvalidTitle",
        "عنوان بخش نامعتبر است");

    public static Error InvalidDisplayOrder => Error.Validation(
        "Section.InvalidDisplayOrder",
        "ترتیب نمایش نامعتبر است");

    public static Error EpisodeNotFound => Error.NotFound(
        "Section.EpisodeNotFound",
        "قسمت مورد نظر یافت نشد");

    public static Error InvalidEpisodeOrder => Error.Validation(
        "Section.InvalidEpisodeOrder",
        "ترتیب قسمت‌ها نامعتبر است");
}
