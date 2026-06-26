using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Roles.Commands.Create
{
    public sealed record CreateRoleCommand(
        string RoleName, List<Guid> PermissionIds) : ICommand<Guid>;
}
