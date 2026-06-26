using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandHandler(
        IUserRepository userRepository)
        : ICommandHandler<DeactivateUserCommand>
    {
        public async Task<Result> Handle(
            DeactivateUserCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
            if (user is null)
                return Error.NotFound("DeactivateUserCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");

            var deactivateResult = user.Deactivate();

            if (deactivateResult.IsFailure)
                return deactivateResult.Error;

            await userRepository.UpdateAsync(user, cancellationToken);
            return Result.Success();

        }
    }
}
