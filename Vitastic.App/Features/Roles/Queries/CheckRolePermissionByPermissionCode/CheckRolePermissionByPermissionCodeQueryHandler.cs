using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Roles.Queries.CheckRolePermission;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermissionByPermissionCode
{
    public sealed class CheckRolePermissionByPermissionCodeQueryHandler(IRoleQueryService repository)
        : IQueryHandler<CheckRolePermissionByPermissionCodeQuery, bool>
    {
        public async Task<Result<bool>> Handle(CheckRolePermissionByPermissionCodeQuery request, CancellationToken cancellationToken)
        {
            Result<RoleId> roleIdResult = RoleId.CreateFrom(request.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            return await repository.HasPermissionAsync(roleIdResult.Value,request.PermissionCode, cancellationToken);
        }
    }
}
