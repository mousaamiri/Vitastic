using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Roles.Commands.AddPermissionToRole;

public sealed record AddPermissionToRoleCommand : ICommand
{

        public Guid RoleId { get; init; }
        public Guid PermissionId { get; init; }

        public AddPermissionToRoleCommand() { }

        public AddPermissionToRoleCommand(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
}

