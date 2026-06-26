using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Commands.UpdateByAdmin
{
    public sealed class UpdateRoleByAdminCommandHandler
        (IRoleRepository repository, IMapper mapper,IPermissionRepository permissionRepository)
        : ICommandHandler<UpdateRoleByAdminCommand>
    {
        public async Task<Result> Handle(UpdateRoleByAdminCommand request, CancellationToken cancellationToken)
        {
            // convert id
            var roleIdResult = RoleId.CreateFrom(request.RoleId);
            if(roleIdResult.IsFailure)
                return roleIdResult.Error;
            // get role
            Role? role = await repository.FindAsync(roleIdResult.Value,cancellationToken);
            if(role is null)
                return Error.NotFound("UpdateRoleNameCommand.RoleNotFound", "چنین نقشی یافت نشد.");

            //prepare role name
            var roleNameResult = RoleName.Create((request.RoleName));
            if (roleNameResult.IsFailure)
                return roleNameResult.Error;
            if (!role.Name.Equals(roleNameResult.Value))
            {
                bool isExist = await repository.NameIsExistAsync(roleNameResult.Value, cancellationToken);
                if (isExist) return Error.Conflict("UpdateRoleNameCommand.Conflict", "این نقش از قبل موجود است.");
            }
            //manage permissions
            List<PermissionId> permissionIds =[];
            foreach (var permissionId in request.PermissionIds)
            {
                var permissionIdResult = PermissionId.CreateFrom(permissionId);
                if (permissionIdResult.IsFailure)
                    return permissionIdResult.Error;
                var permissionIsExist = await permissionRepository.IsExistAsync(permissionIdResult.Value, cancellationToken);
                if (!permissionIsExist)
                    return Error.NotFound("UpdateRole.PermissionNotFound", "مجوز یافت نشد.");
                permissionIds.Add(permissionIdResult.Value);
            }

            Result managerPermissions = role.ManagerPermissions(permissionIds);
            if (managerPermissions.IsFailure)
                return managerPermissions.Error;
            //update role
            role.UpdateName(roleNameResult.Value);
            await repository.UpdateAsync(role, cancellationToken);
            return Result.Success();
        }
    }
}
