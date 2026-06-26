using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Roles.Commands.UpdateByAdmin;

public sealed record UpdateRoleByAdminCommand : ICommand
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = null!;

    public List<Guid> PermissionIds { get; init; }

    public UpdateRoleByAdminCommand() { }
}
