using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandHandler(
        IUserRepository userRepository)
        : ICommandHandler<UpdateProfileCommand>
    {
        public async Task<Result> Handle(
            UpdateProfileCommand command,
            CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            var user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
            if (user is null)
                return Error.NotFound("UpdateProfileCommand.UserNotFound", "کاربر مورد نظر یافت نشد.");
            // Validate FirstName
            FirstName? firstName=null;
            if (command.FirstName != null)
            {
                var firstNameResult = FirstName.Create(command.FirstName);
                if (firstNameResult.IsFailure) return firstNameResult.Error;
                firstName = firstNameResult.Value;
            }

            // Validate LastName
            LastName? lastName = null;
            if (command.LastName != null)
            {
                var lastNameResult=LastName.Create(command.LastName);
                if (lastNameResult.IsFailure) return lastNameResult.Error;
                lastName = lastNameResult.Value;
            }

            // Validate PhoneNumber (optional)
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(command.PhoneNumber);
                if (phoneResult.IsFailure) return phoneResult.Error;
                phoneNumber = phoneResult.Value;
            }
            var updateResult = user.UpdateProfile(firstName, lastName, phoneNumber);

            if (updateResult.IsFailure)
                throw new InvalidOperationException(updateResult.Error.Message);

            await userRepository.UpdateAsync(user, cancellationToken);

            return Result.Success();

        }
    }
}
