using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.ChangeEmail
{
    public sealed class ChangeEmailCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeEmailCommandHandler> logger)
        : ICommandHandler<ChangeEmailCommand>
    {
        public async Task<Result> Handle(
            ChangeEmailCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
            if (user is null)
                return Error.NotFound("ChangeEmailCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");


            var changeResult = user.ChangeEmail(command.NewEmail);
            if (changeResult.IsFailure)
                return changeResult.Error;

            await userRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}