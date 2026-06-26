using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.RemoveRoleFromUser
{
    public sealed class RemoveRoleFromUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
        : ICommandHandler<RemoveRoleFromUserCommand>
    {
        public async Task<Result> Handle(
            RemoveRoleFromUserCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var userIsExist = await userRepository.IsExistAsync(userIdResult.Value, cancellationToken);
            if (!userIsExist)
                return Error.NotFound("RemoveRoleFromUserCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");

            var roleIdResult = RoleId.CreateFrom(command.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            var roleIsExist = await roleRepository.IsExistAsync(roleIdResult.Value, cancellationToken);
            if (!roleIsExist)
                return Error.NotFound("RemoveRoleFromUserCommand.RoleNotFound", "نقش مورد نظر یافت نشد.");

            var connection = await userRoleRepository.FindConnectionAsync(userIdResult.Value,roleIdResult.Value, cancellationToken);
            if (connection  == null)
                return Error.Conflict("RemoveRoleFromUserCommand.ConnectionIsNotExist", "اتصال نقش به کاربر یافت نشده است.");

            await userRoleRepository.DeleteAsync( connection, cancellationToken);
            return Result.Success();
        }
    }
}
