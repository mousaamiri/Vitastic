using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        IUserRepository userRepository)
        : ICommandHandler<ChangePasswordCommand>
    {
        public async Task<Result> Handle(
            ChangePasswordCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
            if (user is null)
                return Error.NotFound("ChangeEmailCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");

            var changeResult = user.ChangePassword(command.CurrentPassword, command.NewPassword);

            if (changeResult.IsFailure)
                throw new InvalidOperationException(changeResult.Error.Message);

            await userRepository.UpdateAsync(user, cancellationToken);
            return Result.Success();
        }
    }
}