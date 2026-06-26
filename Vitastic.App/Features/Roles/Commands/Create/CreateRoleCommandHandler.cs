using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Commands.Create
{
    public sealed class CreateRoleCommandHandler(IRoleRepository repository
        ,IPermissionRepository permissionRepository, IMapper mapper)
        : ICommandHandler<CreateRoleCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // prepare role name
            var roleNameResult = RoleName.Create((request.RoleName));
            if (roleNameResult.IsFailure)
                return roleNameResult.Error;
            bool isExist = await repository.NameIsExistAsync(roleNameResult.Value, cancellationToken);
            if (isExist)
                return Error.Conflict("CreateRoleCommand.Conflict", "این نقش از قبل موجود است.");
            //create role
            var roleCreateResult = Role.Create(request.RoleName);
            if (roleCreateResult.IsFailure)
                return roleCreateResult.Error;
            var role = roleCreateResult.Value;
            //manage role permissions
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

            //add role to db
            await repository.AddAsync(role, cancellationToken);
            return role.Id.Value;
        }
    }
}
