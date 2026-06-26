using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserCommand(
        string Email,
        string ActivationCode) : ICommand;
}
