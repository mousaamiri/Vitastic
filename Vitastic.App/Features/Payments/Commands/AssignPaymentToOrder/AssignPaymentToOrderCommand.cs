using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;

namespace Vitastic.App.Features.Payments.Commands.AssignPaymentToOrder
{
    public sealed record AssignPaymentToOrderCommand(
        Guid TransactionId,
        Guid OrderId) : ICommand;
}
