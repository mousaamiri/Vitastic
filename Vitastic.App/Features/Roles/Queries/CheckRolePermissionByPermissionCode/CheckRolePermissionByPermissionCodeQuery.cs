using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermissionByPermissionCode
{
    /// <summary>
    /// Check if the role has permissions
    /// </summary>
    public sealed record CheckRolePermissionByPermissionCodeQuery(
        Guid RoleId,
        string PermissionCode) : IQuery<bool>;
}
