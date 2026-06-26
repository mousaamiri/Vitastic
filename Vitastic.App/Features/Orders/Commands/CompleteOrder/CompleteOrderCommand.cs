using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Features.Orders.Commands.CompleteOrder;

public sealed record CompleteOrderCommand(
    Guid OrderId,Guid TransactionId) : ICommand;
