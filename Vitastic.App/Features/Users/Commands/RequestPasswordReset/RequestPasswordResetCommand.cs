using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.RequestPasswordReset;

public sealed record RequestPasswordResetCommand(string Email,
    string CallbackBaseUrl)  : ICommand;
