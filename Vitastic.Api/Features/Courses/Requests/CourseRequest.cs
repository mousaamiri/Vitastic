using Vitastic.Api.Features.Courses.Responses;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.Api.Features.Courses.Requests;

public sealed record AddCourseSectionRequest(
    string Title,
    int DisplayOrder
);

public sealed record AddCourseEpisodeRequest(
    string Title,
    TimeSpan Duration,
    decimal Price,
    IFormFile VideoFile
);

public sealed record AddCourseTagRequest(
    Guid TagId
);

public sealed record AddCourseCategoryRequest(
    Guid CategoryId
);
public sealed record ChangeCourseInstructorRequest(
    Guid InstructorId
);

public sealed record CreateCourseRequest(
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelResponse Level,
    Guid InstructorId
);

public sealed record CreateCourseByAdminRequest(
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelResponse Level,
    Guid InstructorId,
    IReadOnlyCollection<Guid>? TagIds,
    IReadOnlyCollection<Guid>? CategoryIds,
    bool HasCertificate = false,
    IFormFile? ImageFile = null,
    IFormFile? ThumbnailFile = null,
    IFormFile? DemoVideoFile = null);

public sealed record UpdateCourseByAdminRequest(
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelResponse Level,
    Guid InstructorId,
    IReadOnlyCollection<Guid>? TagIds,
    IReadOnlyCollection<Guid>? CategoryIds,
    bool HasCertificate = false,
    IFormFile? ImageFile = null,
    IFormFile? ThumbnailFile = null,
    IFormFile? DemoVideoFile = null);

public sealed record UpsertCourseSections(
    List<SectionResponse> Sections);

public sealed record ReorderCourseSectionsRequest(
    IReadOnlyCollection<Guid> OrderedSectionIds
);

public sealed record SetCourseCategoriesRequest(
    IReadOnlyCollection<Guid> CategoryIds
);

public sealed record SetCourseDemoVideoRequest(
    IFormFile VideoFile
);

public sealed record SetCourseImageRequest(
    IFormFile ImageFile
);

public sealed record SetCourseTagsRequest(
    IReadOnlyCollection<Guid> TagIds
);

public sealed record UnpublishCourseRequest(
    Guid CourseId
);

public sealed record UpdateCourseDescriptionRequest(
    string Description,
    string ShortDescription
);

public sealed record ChangeOrderStatusByAdminRequest(OrderStatusDto Status, string? AdminNote);

public record UpdateCourseDetailsRequest(
    string? Title, // nullable = اگر نفرستاده یعنی تغییر نده
    string? Description,
    string? ShortDescription);

public sealed record ChangeCourseLevelRequest(
    CourseLevelResponse Level
);

public sealed record UpdateCourseEpisodeRequest(
    string? Title = null,
    TimeSpan? Duration = null,
    decimal? Price = null,
    string? Currency = null,
    IFormFile? VideoFile = null
);

public sealed record UpdateSectionTitleRequest(
    string Title
);

public sealed record UpdateCourseSlugRequest(
    string Slug
);

public sealed record UpdateCourseTitleRequest(
    string Title
);

public sealed record SearchCoursesParameters(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? InstructorId = null,
    Guid? CategoryId = null,
    CourseLevelResponse? Level = null,
    CourseStatusResponse? Status = null,
    bool? IsPublished = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    CourseSortByRequest? SortBy = null, // newest, popular, cheapest, expensive
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? HasCertificate = null,
    bool? IsFree = null
);
#region Sort Enum
public enum CourseSortByRequest
{
    Newest = 0,                 // جدیدترین
    Oldest = 1,           // قدیمی‌ترین
    PriceAscending = 2,         // ارزان‌ترین
    PriceDescending = 3,        // گران‌ترین
    BestSelling = 4,            // پرفروش‌ترین
    HighestRated = 5,       // بیشترین امتیاز
    MostPopular = 6             // محبوب‌ترین (ترکیب فروش + امتیاز)
}
#endregion
