using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses.Enums;

namespace Vitastic.App.Features.Courses.Queries.Search;

public sealed record SearchCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? InstructorId = null,
    Guid? CategoryId = null,
    CourseLevelDto? Level = null,
    CourseStatusDto? Status = null,
    DateTimeOffset? FromDate=null,
    DateTimeOffset? ToDate=null,
    CourseSortBy? SortBy = null, // newest, popular, cheapest, expensive
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? HasCertificate = null,
    bool? IsFree = null,
    Guid? UserId=null,
    string? SessionId=null
) : IQuery<PaginatedResult<SimpleCourseDto>>;
