using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserStatusCommandHandler(
        IUserRepository userRepository)
        : ICommandHandler<ActivateUserCommand>
    {
        public async Task<Result> Handle(
            ActivateUserCommand command,
            CancellationToken cancellationToken)
        {
            Result<Email> emailResult = Email.Create(command.Email);
            if(emailResult.IsFailure)
                return emailResult.Error;
            var emailIsExist = await userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
            if (!emailIsExist)
                return Error.NotFound("ChangeEmailCommand.UserNotFoundByEmail", "کاربری با این ایمیل یافت نشد.");
            User? user = await userRepository.FindUserByActiveCodeAsync(command.ActivationCode, cancellationToken);
            if (user is null)
                return Error.NotFound("ChangeEmailCommand.UserNotFoundByActivateCode", "کاربری با این کد فعالسازی یافت نشد.");

            var activateResult = user.Activate(command.ActivationCode);
            if (activateResult.IsFailure)
                return  activateResult.Error;

            await userRepository.UpdateAsync(user, cancellationToken);
            return Result.Success();

        }
    }
}
