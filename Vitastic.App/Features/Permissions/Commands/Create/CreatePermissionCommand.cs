using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Permissions.Commands.Create;

public record CreatePermissionCommand(string Code,string Description):ICommand<Guid>;
