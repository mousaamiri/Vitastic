using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermission
{
    public sealed class CheckRolePermissionQueryHandler(IRoleQueryService repository) : IQueryHandler<CheckRolePermissionQuery, bool>
    {
        public async Task<Result<bool>> Handle(CheckRolePermissionQuery request, CancellationToken cancellationToken)
        {
            Result<RoleId> roleIdResult = RoleId.CreateFrom(request.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            Result<PermissionId> permissionId = PermissionId.CreateFrom(request.PermissionId);
            if(permissionId.IsFailure)
                    return permissionId.Error;
            return await repository.HasPermissionAsync(roleIdResult.Value,permissionId.Value, cancellationToken);
        }
    }
}
