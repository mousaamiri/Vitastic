using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.App.Features.Courses.Queries.Search;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.GetMyCourses
{
    public sealed class GetMyCoursesCoursesQueryHandler(ICourseQueryService courseQueryService, IMapper mapper)
        : IQueryHandler<GetMyCoursesCoursesQuery, PaginatedResult<SimpleCourseDto>>
    {
        public async Task<Result<PaginatedResult<SimpleCourseDto>>> Handle(GetMyCoursesCoursesQuery request,
            CancellationToken cancellationToken)
        {
            var userId =UserId.CreateFrom(request.UserId);
            if(userId.IsFailure)
                return userId.Error;
            (IReadOnlyList<SimpleCourseDto> courses, var totalCount)
                = await courseQueryService.GetMyCoursesAsync(
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize,
                    request.InstructorId,
                    request.CategoryId,
                    (CourseLevel?)request.Level,
                    (CourseStatus?)request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.SortBy,
                    request.MinPrice,
                    request.MaxPrice,
                    request.HasCertificate,
                    request.IsFree,
                    userId.Value,
                    cancellationToken);
            return new PaginatedResult<SimpleCourseDto>(courses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
