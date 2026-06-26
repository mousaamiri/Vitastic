namespace Vitastic.Web.Models.DTOs.Course;

public sealed record UpsertCreateCourseRequest(
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelDto Level,
    Guid InstructorId,
    IReadOnlyCollection<Guid>? TagIds,
    IReadOnlyCollection<Guid>? CategoryIds,
    bool HasCertificate = false,
    IFormFile? ImageFile = null,
    IFormFile? ThumbnailFile = null,
    IFormFile? DemoVideoFile = null
);
