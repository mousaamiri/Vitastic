using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.Search
{
    public sealed class SearchCoursesQueryHandler(ICourseQueryService courseQueryService, IMapper mapper)
        : IQueryHandler<SearchCoursesQuery, PaginatedResult<SimpleCourseDto>>
    {
        public async Task<Result<PaginatedResult<SimpleCourseDto>>> Handle(SearchCoursesQuery request,
            CancellationToken cancellationToken)
        {
            UserId? userId = null;
            if (request.UserId != null)
            {
                var userIdResult =UserId.CreateFrom(request.UserId.Value);
                userId = userIdResult.Value;
            }
            (IReadOnlyList<SimpleCourseDto> courses, var totalCount)
                = await courseQueryService.SearchAsync(request.SearchTerm,
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
                    userId,
                    request.SessionId,
                    cancellationToken);
            return new PaginatedResult<SimpleCourseDto>(courses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
