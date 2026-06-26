using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Commands.Update
{
    public sealed class UpdatePermissionCommandHandler(IPermissionRepository repository):ICommandHandler<UpdatePermissionCommand>
    {
        public async Task<Result> Handle(UpdatePermissionCommand request, CancellationToken token)
        {
            var code =  request.Code?.Trim();
            var description = request.Description?.Trim();
            Result<PermissionId> permissionIdResult =  PermissionId.CreateFrom(request.Id);
            if (permissionIdResult.IsFailure)
                return permissionIdResult.Error;
            Permission? permission = await repository.FindAsync(permissionIdResult.Value,token);
            if (permission is null)
                return Error.NotFound("UpdatePermissionCommand.PermissionNotFound", "مجوزی با این شناسه یافت نشد.");
            if (!string.IsNullOrEmpty(code))
            {
                var isExist = await repository.PermissionIsExistedAsync(code,token);
                if (isExist)
                    return Error.Conflict("CreatePermissionCommand.ThisCodeIsExisted", "قبلا یک مجوز با این کد ثبت شده است. ");
                var result = permission.UpdateCode(code);
                if (result.IsFailure)
                    return result.Error;
            }

            if (!string.IsNullOrEmpty(request.Description?.Trim()))
            {
                var result = permission.UpdateDescription(request.Description);
                if (result.IsFailure)
                    return result.Error;
            }
            await repository.UpdateAsync(permission, token);
            return Result.Success();
        }
    }
}