using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.GetById;

public sealed class GetCourseByIdQueryHandler(ICourseQueryService courseQuery, IMapper mapper)
    : IQueryHandler<GetCourseByIdQuery, CourseDto>
{

    public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {

        UserId? userId = null;
        if (request.UserId != null)
        {
            Result<UserId> userIdResult =UserId.CreateFrom(request.UserId.Value);
            userId = userIdResult.Value;
        }
        Result<CourseId> courseId = CourseId.CreateFrom(request.CourseId);
        if(courseId.IsFailure)
            return courseId.Error;
        CourseDto? course = await courseQuery.GetWithSectionsByCourseIdAsync(courseId.Value,userId,request.SessionId, cancellationToken);

        if (course is null)
            return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

        return mapper.Map<CourseDto>(course);
    }
}
