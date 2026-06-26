using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Roles.Commands.RemovePermissionFromRole
{
    public sealed record RemovePermissionFromRoleCommand(
        Guid RoleId,
        Guid PermissionId) : ICommand;
}
