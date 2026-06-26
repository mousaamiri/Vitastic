using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.AddCustomerNote
{
    public sealed class AddCustomerNoteCommandHandler(IOrderRepository orderRepository) : ICommandHandler<AddCustomerNoteCommand>
    {
        public async Task<Result> Handle(AddCustomerNoteCommand request, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("AddCustomerNoteCommand.OrderNotFound", "سفارشی با این شناسه یافت نشد. ");

            order.SetAdminNote(request.Note);

            await orderRepository.UpdateAsync(order, cancellationToken);

            return Result.Success();
        }
    }
}
