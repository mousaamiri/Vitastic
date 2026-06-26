using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.List
{
    public sealed class CoursesListQueryHandler(ICourseQueryService courseQueryService, IMapper mapper)
        : IQueryHandler<CoursesListQuery, PaginatedResult<SimpleCourseDto>>
    {
        public async Task<Result<PaginatedResult<SimpleCourseDto>>> Handle(CoursesListQuery request, CancellationToken cancellationToken)
        {
            UserId? userId = null;
            if (request.UserId != null)
            {
                var userIdResult =UserId.CreateFrom(request.UserId.Value);
                userId = userIdResult.Value;
            }
            (IReadOnlyList<SimpleCourseDto> courses, int totalCount)
             = await courseQueryService.GetPagedAsync(request.PageNumber, request.PageSize,userId,request.SessionId, cancellationToken);
            return new PaginatedResult<SimpleCourseDto>(courses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
