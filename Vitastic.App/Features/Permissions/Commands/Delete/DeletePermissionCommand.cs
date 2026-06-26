using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Permissions.Commands.Delete
{
    public record DeletePermissionCommand(Guid Id):ICommand;
}