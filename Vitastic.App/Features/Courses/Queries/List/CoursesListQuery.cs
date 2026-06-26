using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.List;

public sealed record CoursesListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? UserId = null,
    string? SessionId=null) : IQuery<PaginatedResult<SimpleCourseDto>>;
