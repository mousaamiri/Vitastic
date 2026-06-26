using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.RemoveRoleFromUser
{
    public sealed record RemoveRoleFromUserCommand(
        Guid UserId,
        Guid RoleId) : ICommand;
}