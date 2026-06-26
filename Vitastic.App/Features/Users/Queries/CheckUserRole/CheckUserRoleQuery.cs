using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Queries.CheckUserRole
{
    public sealed record CheckUserRoleQuery(
        Guid UserId,
        Guid RoleId) : IQuery<bool>;
}
