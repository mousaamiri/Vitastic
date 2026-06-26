using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Commands.Create
{
    public sealed class CreatePermissionCommandHandler(IPermissionRepository repository):ICommandHandler<CreatePermissionCommand,Guid>
    {
        public async Task<Result<Guid>> Handle(CreatePermissionCommand request, CancellationToken token)
        {
            var isExist = await repository.PermissionIsExistedAsync(request.Code,token);
            if (isExist)
                return Error.Conflict("CreatePermissionCommand.ThisCodeIsExisted", "قبلا یک مجوز با این کد ثبت شده است. ");
            Result<Permission> permissionResult = Permission.Create(request.Code, request.Description);
            if (permissionResult.IsFailure)
                return permissionResult.Error;
            await repository.AddAsync(permissionResult.Value, token);
            return permissionResult.Value.Id.Value;
        }
    }
}