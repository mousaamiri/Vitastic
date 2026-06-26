using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.App.Features.Users.Commands.Login;

public sealed record LoginUserCommand(string Identifier, string Password) : ICommand<JwtResult>;
