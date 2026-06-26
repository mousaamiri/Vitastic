using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.GetInstructorCourses
{
    public sealed record GetInstructorCoursesQuery(
        Guid InstructorId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<PaginatedResult<SimpleCourseDto>>;
}
