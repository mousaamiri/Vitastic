using MediatR;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.ClearOrderItems;

public sealed record ClearOrderItemsCommand(
    Guid OrderId) : ICommand;
