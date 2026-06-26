using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Commands.RemovePermissionFromRole
{
    public sealed class RemovePermissionFromRoleCommandHandler
        (IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository)
        : ICommandHandler<RemovePermissionFromRoleCommand>
    {
        public async Task<Result> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var roleIdResult = RoleId.CreateFrom(request.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            var roleIsExisted = await roleRepository.RoleIsExistedAsync(roleIdResult.Value, cancellationToken);
            if (!roleIsExisted)
                return Error.NotFound("RemovePermissionFromRoleCommand.RoleNotFound", "نقش مورد نظر یافت نشد.");

            Result<PermissionId> permissionIdResult = PermissionId.CreateFrom(request.PermissionId);
            if (permissionIdResult.IsFailure)
                return permissionIdResult.Error;
            var permissionIsExist = await permissionRepository.PermissionIsExistedAsync(permissionIdResult.Value, cancellationToken);
            if (!permissionIsExist)
                return Error.NotFound("RemovePermissionFromRoleCommand.PermissionIsNotExist", "مجوز مورد نظر یافت نشد.");

            var relation =
                await rolePermissionRepository.FindAsync(roleIdResult.Value,permissionIdResult.Value,cancellationToken);
            if(relation  == null)
                return Error.Conflict("RemovePermissionFromRoleCommand.RelationIsNotExist", "اتصال نقش به مجوز از قبل براقرا نیست.");

            await rolePermissionRepository.DeleteAsync(relation, cancellationToken);
            return Result.Success();
        }
    }
}
