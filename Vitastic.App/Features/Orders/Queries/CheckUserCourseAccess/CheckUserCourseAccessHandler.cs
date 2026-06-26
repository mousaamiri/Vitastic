using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.CheckUserCourseAccess
{
    public sealed class CheckUserCourseAccessHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<CheckUserCourseAccessQuery, bool>
    {
        public async Task<Result<bool>> Handle(
            CheckUserCourseAccessQuery query,
            CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            Result<CourseId> courseIdResult = CourseId.CreateFrom(query.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            return await orderQueryService.UserHasCourseAccessAsync(
                userIdResult.Value,
                courseIdResult.Value,
                cancellationToken);
        }
    }
}
