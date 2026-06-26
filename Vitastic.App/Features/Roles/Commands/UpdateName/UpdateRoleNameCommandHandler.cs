using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Commands.UpdateName
{
    public sealed class UpdateRoleNameCommandHandler
        (IRoleRepository repository, IMapper mapper,IPermissionRepository permissionRepository)
        : ICommandHandler<UpdateRoleNameCommand>
    {
        public async Task<Result> Handle(UpdateRoleNameCommand request, CancellationToken cancellationToken)
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
            bool isExist = await repository.NameIsExistAsync(roleNameResult.Value, cancellationToken);
            if (isExist)
                return Error.Conflict("UpdateRoleNameCommand.Conflict", "این نقش از قبل موجود است.");

            //update role
            role.UpdateName(roleNameResult.Value);
            await repository.UpdateAsync(role, cancellationToken);
            return Result.Success();
        }
    }
}
