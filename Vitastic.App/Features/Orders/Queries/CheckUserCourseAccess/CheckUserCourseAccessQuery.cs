using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Queries.CheckUserCourseAccess
{
    public sealed record CheckUserCourseAccessQuery(Guid UserId, Guid CourseId) : IQuery<bool>;
}
