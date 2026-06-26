using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.GetInstructorCourses
{
    public sealed class GetInstructorCoursesQueryHandler(ICourseQueryService courseService, IMapper mapper)
        : IQueryHandler<GetInstructorCoursesQuery, PaginatedResult<SimpleCourseDto>>
    {
        public async Task<Result<PaginatedResult<SimpleCourseDto>>> Handle(GetInstructorCoursesQuery request,
            CancellationToken cancellationToken)
        {
            var instructorId = InstructorId.CreateFrom(request.InstructorId);
            if (instructorId.IsFailure)
                return instructorId.Error;
            (IReadOnlyList<SimpleCourseDto> courses, int totalCount) = await courseService.GetInstructorCoursesAsync(
                instructorId.Value,
                request.PageNumber, request.PageSize, cancellationToken);
            return new PaginatedResult<SimpleCourseDto>(courses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
