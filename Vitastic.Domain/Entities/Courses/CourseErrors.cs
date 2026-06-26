using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses;

public static class CourseErrors
{
    public static Error InvalidTitle => Error.Validation(
        "Course.InvalidTitle",
        "عنوان دوره نامعتبر است");

    public static Error InvalidDescription => Error.Validation(
        "Course.InvalidDescription",
        "توضیحات دوره نامعتبر است");

    public static Error InvalidShortDescription => Error.Validation(
        "Course.InvalidShortDescription",
        "توضیحات کوتاه دوره نامعتبر است");

    public static Error InvalidSlug => Error.Validation(
        "Course.InvalidSlug",
        "نامک (Slug) نامعتبر است");

    public static Error InvalidPrice => Error.Validation(
        "Course.InvalidPrice",
        "قیمت دوره نامعتبر است");

    public static Error InvalidInstructor => Error.Validation(
        "Course.InvalidInstructor",
        "مدرس نامعتبر است");

    public static Error CurrencyMismatch => Error.Validation(
        "Course.CurrencyMismatch",
        "واحد پول مطابقت ندارد");

    public static Error InvalidImageName => Error.Validation(
        "Course.InvalidImageName",
        "نام فایل تصویر نامعتبر است");

    public static Error InvalidVideoName => Error.Validation(
        "Course.InvalidVideoName",
        "نام فایل ویدیو نامعتبر است");

    public static Error SectionNotFound => Error.NotFound(
        "Course.SectionNotFound",
        "بخش مورد نظر یافت نشد");

    public static Error SectionTitleDuplicate => Error.Conflict(
        "Course.SectionTitleDuplicate",
        "بخشی با این عنوان قبلاً اضافه شده است");

    public static Error InvalidSectionOrder => Error.Validation(
        "Course.InvalidSectionOrder",
        "ترتیب بخش‌ها نامعتبر است");

    public static Error InvalidTag => Error.Validation(
        "Course.InvalidTag",
        "برچسب نامعتبر است");

    public static Error TagAlreadyAdded => Error.Conflict(
        "Course.TagAlreadyAdded",
        "این برچسب قبلاً اضافه شده است");

    public static Error TagNotFound => Error.NotFound(
        "Course.TagNotFound",
        "برچسب یافت نشد");

    public static Error InvalidCategory => Error.Validation(
        "Course.InvalidCategory",
        "دسته‌بندی نامعتبر است");

    public static Error CategoryAlreadyAdded => Error.Conflict(
        "Course.CategoryAlreadyAdded",
        "این دسته‌بندی قبلاً اضافه شده است");

    public static Error CategoryNotFound => Error.NotFound(
        "Course.CategoryNotFound",
        "دسته‌بندی یافت نشد");

    public static Error CertificateAlreadyEnabled => Error.Conflict(
        "Course.CertificateAlreadyEnabled",
        "گواهینامه قبلاً فعال شده است");

    public static Error CertificateAlreadyDisabled => Error.Conflict(
        "Course.CertificateAlreadyDisabled",
        "گواهینامه قبلاً غیرفعال شده است");

    public static Error CannotModifyPublishedCourse => Error.Validation(
        "Course.CannotModifyPublishedCourse",
        "دوره منتشر شده قابل ویرایش نیست");

    public static Error CannotChangeSlugAfterPublish => Error.Validation(
        "Course.CannotChangeSlugAfterPublish",
        "پس از انتشار دوره، نامک قابل تغییر نیست");

    public static Error CannotChangeInstructorAfterPublish => Error.Validation(
        "Course.CannotChangeInstructorAfterPublish",
        "پس از انتشار دوره، مدرس قابل تغییر نیست");

    public static Error AlreadyPublished => Error.Conflict(
        "Course.AlreadyPublished",
        "دوره قبلاً منتشر شده است");

    public static Error NotPublished => Error.Validation(
        "Course.NotPublished",
        "دوره منتشر نشده است");

    public static Error AlreadyArchived => Error.Conflict(
        "Course.AlreadyArchived",
        "دوره قبلاً بایگانی شده است");

    public static Error CannotPublishWithoutSections => Error.Validation(
        "Course.CannotPublishWithoutSections",
        "دوره بدون بخش قابل انتشار نیست");

    public static Error CannotPublishWithEmptySections => Error.Validation(
        "Course.CannotPublishWithEmptySections",
        "همه بخش‌ها باید حداقل یک قسمت داشته باشند");
    public static Error InvalidCurrency => Error.Validation(
        "Course.InvalidCurrency",
        "واحد پول نامعتبر است");
    public static Error IsFailure => Error.Validation(
        "Course.IsFailure",
        "خطا در ایجاد دوره");
    public static Error CannotModifyArchivedCourse => Error.Validation(
        "Course.CannotModifyArchivedCourse",
        "دوره بایگانی شده قابل ویرایش نیست");

    public static Error CannotRemoveSectionThatHasEpisode =>
        Error.Conflict("Course.CannotRemoveSectionThatHasEpisode",
            "نمیتواند بخشی که شامل تعدادی اپیزود هست را حذف کرد. لطفا ابتدا اپیزود ها را حذف کنید و سپس اقدام کنید.");
   public static Error SlugAlreadyExists =>
        Error.Conflict("Course.SlugAlreadyExists",
            "آدرس این دوره قبلا ثبت شده است.");

   public static readonly Error SectionsCannotBeEmpty =
       Error.Validation("Course.SectionsCannotBeEmpty", "دوره باید حداقل یک بخش داشته باشد");

   public static readonly Error DuplicateDisplayOrder =
       Error.Validation("Course.DuplicateDisplayOrder", "ترتیب نمایش بخش‌ها نباید تکراری باشد");

   public static readonly Error SectionDoesNotBelongToCourse =
       Error.Validation("Course.SectionDoesNotBelongToCourse", "بخش متعلق به این دوره نیست");


}
