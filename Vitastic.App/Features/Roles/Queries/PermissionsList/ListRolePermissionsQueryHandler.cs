using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.PermissionsList;

public class ListRolePermissionsQueryHandler(IRoleQueryService roleService)
    : IQueryHandler<ListRolePermissionsQuery, List<RolePermissionDto>>
{
    public async Task<Result<List<RolePermissionDto>>> Handle(ListRolePermissionsQuery query,
        CancellationToken cancellationToken)
    {
        var roleIdResult = RoleId.CreateFrom(query.RoleId);
        if (roleIdResult.IsFailure)
            return roleIdResult.Error;
        List<RolePermissionDto> rolePermissions =
            await roleService.GetRolePermissionsAsync(roleIdResult.Value, cancellationToken);
        return rolePermissions;
    }
}
