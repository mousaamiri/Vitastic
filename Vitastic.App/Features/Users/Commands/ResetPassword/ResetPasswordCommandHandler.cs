using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(
        IUserRepository userRepository) : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await userRepository.GetByResetTokenAsync(request.Token, cancellationToken);
            if (user is null)
                return UserErrors.InvalidResetToken;

            Result result = user.ResetPasswordWithToken(request.Token, request.NewPassword);
            return result.IsFailure ? result.Error : Result.Success();
        }
    }
}