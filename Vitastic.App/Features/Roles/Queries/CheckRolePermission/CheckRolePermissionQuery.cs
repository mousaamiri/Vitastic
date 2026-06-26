using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermission
{
    /// <summary>
    /// Check if the role has permissions
    /// </summary>
    public sealed record CheckRolePermissionQuery(
        Guid RoleId,
        Guid PermissionId) : IQuery<bool>;
}
