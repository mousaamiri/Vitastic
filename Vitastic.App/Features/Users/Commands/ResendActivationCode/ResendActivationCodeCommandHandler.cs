using Microsoft.Extensions.Options;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Settings;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ResendActivationCode
{
    public sealed class ResendActivationCodeCommandHandler(
        IUserRepository userRepository,
        IOptions<ClientSettings> clientSettings) : ICommandHandler<ResendActivationCodeCommand>
    {
        public async Task<Result> Handle(ResendActivationCodeCommand request, CancellationToken cancellationToken)
        {
            // Validation in Command Handler
            if (!clientSettings.Value.IsAllowedUrl(request.CallbackBaseUrl))
                return Error.Validation("CallbackUrl", "آدرس نامعتبر است");

            Result<Email> emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            User? user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (user is null)
                return UserErrors.NotFound;

            Result result = user.RegenerateActivationCode(request.CallbackBaseUrl);
            return result.IsFailure ? result.Error : Result.Success();
        }
    }
}
