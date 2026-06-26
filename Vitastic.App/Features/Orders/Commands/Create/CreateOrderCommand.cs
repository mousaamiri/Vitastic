using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.Create;

public sealed record CreateOrderCommand(Guid UserId) : ICommand<Guid>;
