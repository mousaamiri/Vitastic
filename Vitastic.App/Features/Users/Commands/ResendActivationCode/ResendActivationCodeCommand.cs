using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.ResendActivationCode;
public sealed record ResendActivationCodeCommand(string Email,
    string CallbackBaseUrl)  : ICommand;
