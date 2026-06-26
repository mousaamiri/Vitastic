using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Roles.Commands.UpdateName;

public sealed record UpdateRoleNameCommand : ICommand
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = null!;

    public UpdateRoleNameCommand() { }
}
