using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.AddItemToOrder;

public sealed record AddItemToOrderCommand(
    Guid OrderId,
    Guid CourseId) : ICommand<Guid>;
