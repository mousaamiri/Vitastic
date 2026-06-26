using Microsoft.Extensions.Options;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Settings;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.RequestPasswordReset
{
    public sealed class RequestPasswordResetCommandHandler(
        IUserRepository userRepository,
        IOptions<ClientSettings> clientSettings) : ICommandHandler<RequestPasswordResetCommand>
    {
        public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
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

            Result result = user.RequestPasswordReset(request.CallbackBaseUrl);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}
