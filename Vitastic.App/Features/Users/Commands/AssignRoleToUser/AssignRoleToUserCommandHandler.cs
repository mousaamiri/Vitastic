using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.AssignRoleToUser
{
    public sealed class AssignRoleToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
        : ICommandHandler<AssignRoleToUserCommand>
    {
        public async Task<Result> Handle(
            AssignRoleToUserCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var userIsExist = await userRepository.IsExistAsync(userIdResult.Value, cancellationToken);
            if (!userIsExist)
                return Error.NotFound("ChangeEmailCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");

            var roleIdResult = RoleId.CreateFrom(command.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            var roleIsExist = await roleRepository.IsExistAsync(roleIdResult.Value, cancellationToken);
            if (!roleIsExist)
                return Error.NotFound("ChangeEmailCommand.RoleNotFound", "نقش مورد نظر یافت نشد.");

            var isUserInRole = await userRoleRepository.IsUserInRoleAsync(userIdResult.Value,roleIdResult.Value, cancellationToken);
            if (isUserInRole)
                return Error.Conflict("ChangeEmailCommand.ConnectionAlreadyExist", "اتصال نقش به کاربر قبلا انجام شده است.");

            await userRoleRepository.AddAsync( UserRole.Create(roleIdResult.Value,userIdResult.Value), cancellationToken);
            return Result.Success();

        }
    }
}
