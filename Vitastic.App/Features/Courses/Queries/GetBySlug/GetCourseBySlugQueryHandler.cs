using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Queries.GetBySlug
{
    public sealed class GetCourseBySlugQueryHandler(ICourseQueryService courseQuery, IMapper mapper) : IQueryHandler<GetCourseBySlugQuery, CourseDto>
    {
        public async Task<Result<CourseDto>> Handle(GetCourseBySlugQuery request, CancellationToken cancellationToken)
        {
            UserId? userId = null;
            if (request.UserId != null)
            {
                var userIdResult =UserId.CreateFrom(request.UserId.Value);
                userId = userIdResult.Value;
            }
            var slugResult = Slug.Create(request.Slug);
            if (slugResult.IsFailure)
                return slugResult.Error;
            CourseDto? course = await courseQuery.GetBySlugAsync(slugResult.Value,userId,request.SessionId, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            return course;
        }
    }
}
