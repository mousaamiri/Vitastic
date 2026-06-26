using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Commands.Delete
{
    public sealed class DeletePermissionCommandHandler(IPermissionRepository repository):ICommandHandler<DeletePermissionCommand>
    {
        public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken token)
        {
            Result<PermissionId> permissionIdResult =  PermissionId.CreateFrom(request.Id);
            if (permissionIdResult.IsFailure)
                return permissionIdResult.Error;
            Permission? permission = await repository.FindAsync(permissionIdResult.Value,token);
            if (permission is null)
                return Error.NotFound("DeletePermissionCommand.PermissionNotFound", "مجوزی با این شناسه یافت نشد.");
            await repository.DeleteAsync(permission, token);
            return Result.Success();
        }
    }
}