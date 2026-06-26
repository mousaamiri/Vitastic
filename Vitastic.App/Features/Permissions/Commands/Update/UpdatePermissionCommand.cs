using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Permissions.Commands.Update
{
    public record UpdatePermissionCommand(Guid Id,string? Code,string? Description):ICommand;
}