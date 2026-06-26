using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.ChangeEmail
{
    public sealed record ChangeEmailCommand(
        Guid UserId,
        string NewEmail) : ICommand;
}