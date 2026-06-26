using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.Register
{
    public sealed record RegisterUserCommand(
        string UserName,
        string Email,
        string Password,
        string CallbackBaseUrl)  : ICommand<Guid>;
}
