using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface ICourseQueryService
{
    Task<CourseDto?> GetBySlugAsync(Slug slug, UserId? userId, string? sessionId,
        CancellationToken cancellationToken = default);
    Task<CourseDto?> GetWithSectionsByCourseIdAsync(CourseId id, UserId? userId, string? sessionId,
        CancellationToken token = default);
    Task<CourseDto?> GetByIdAsync(CourseId value,UserId? userId, CancellationToken token=default);

    Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)>
    GetInstructorCoursesAsync(InstructorId id,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> GetPagedAsync(int pageNumber,
        int pageSize,
        UserId? userId,
        string? sessionId,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> SearchAsync(string searchTerm, int pageNumber,
        int pageSize, Guid? instructorId,
        Guid? categoryId, CourseLevel? level, CourseStatus? status,
        DateTimeOffset? fromDate, DateTimeOffset? toDate, CourseSortBy? sortBy,
        decimal? minPrice, decimal? maxPrice, bool? hasCertificate,
        bool? isFree,
        UserId? userId,
        string? sessionId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> GetMyCoursesAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        Guid? instructorId,
        Guid? categoryId,
        CourseLevel? level,
        CourseStatus? status,
        DateTimeOffset? fromDate,
        DateTimeOffset? toDate,
        CourseSortBy? sortBy,
        decimal? minPrice,
        decimal? maxPrice,
        bool? hasCertificate,
        bool? isFree,
        UserId userId,
        CancellationToken cancellationToken = default);

}
