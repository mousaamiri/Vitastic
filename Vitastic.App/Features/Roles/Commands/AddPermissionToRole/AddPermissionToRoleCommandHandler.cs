using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Commands.AddPermissionToRole
{
    public sealed class AddPermissionToRoleCommandHandler
        (IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository)
        : ICommandHandler<AddPermissionToRoleCommand>
    {

        public async Task<Result> Handle(AddPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var roleIdResult = RoleId.CreateFrom(request.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            var roleIsExisted = await roleRepository.RoleIsExistedAsync(roleIdResult.Value, cancellationToken);
            if (!roleIsExisted)
                return Error.NotFound("AddPermissionToRoleCommand.RoleNotFound", "نقش مورد نظر یافت نشد.");

            Result<PermissionId> permissionIdResult = PermissionId.CreateFrom(request.PermissionId);
            if (permissionIdResult.IsFailure)
                return permissionIdResult.Error;
            var permissionIsExist = await permissionRepository.PermissionIsExistedAsync(permissionIdResult.Value, cancellationToken);
            if (!permissionIsExist)
                return Error.NotFound("AddPermissionToRoleCommand.PermissionIsNotExist", "مجوز مورد نظر یافت نشد.");

            var rolePermissionConnection = RolePermission.Create(roleIdResult.Value, permissionIdResult.Value);
            var relationIsExist = await rolePermissionRepository.RelationIsExist(rolePermissionConnection,cancellationToken);
            if(relationIsExist)
                return Error.Conflict("RemovePermissionFromRoleCommand.RelationIsExist", "اتصال نقش به مجوز از قبل براقرا است.");

            await rolePermissionRepository.AddAsync(rolePermissionConnection, cancellationToken);
            return Result.Success();
        }
    }
}
