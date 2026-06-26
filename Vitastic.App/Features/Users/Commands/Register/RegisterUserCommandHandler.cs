using Microsoft.Extensions.Options;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Settings;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.Register
{
    public sealed class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IOptions<ClientSettings> clientSettings)
        : ICommandHandler<RegisterUserCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(
            RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            // Validation in Command Handler
            if (!clientSettings.Value.IsAllowedUrl(command.CallbackBaseUrl))
                return Error.Validation("CallbackUrl", "آدرس نامعتبر است");

            Result<Email> emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
                return emailResult.Error;
            var isExistEmail = await userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
            if (isExistEmail)
                return Error.Conflict("RegisterUser.EmailIsExist","این ایمیل قبلا ثبت شده است.");

            Result<UserName> userNameResult = UserName.Create(command.UserName);
            if (emailResult.IsFailure)
                return emailResult.Error;
            var isExistUserName = await userRepository.ExistsByUserNameAsync(userNameResult.Value, cancellationToken);
            if (isExistUserName)
                return Error.Conflict("RegisterUser.,UserNameIsExist","این نام کاربری قبلا ثبت شده است.");

            Result<Password> passwordResult = Password.Create(command.Password);
            if (passwordResult.IsFailure)
                return passwordResult.Error;

            Result<User> userResult = User.Register(
                userNameResult.Value,
                emailResult.Value,
                passwordResult.Value,
                command.CallbackBaseUrl);

            if (userResult.IsFailure)
                throw new InvalidOperationException(userResult.Error.Message);

            User user = userResult.Value;

            await userRepository.AddAsync(user, cancellationToken);

            return user.Id.Value;
        }
    }
}
