using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.DeactivateUser
{
    public sealed record DeactivateUserCommand(
        Guid UserId) : ICommand;
}
