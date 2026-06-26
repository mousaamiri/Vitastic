using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Orders.Commands.AddCustomerInfo;

public record AddCustomerInformationCommand(Guid OrderId,string FullName):ICommand;

public sealed class AddCustomerInformationCommandValidation : AbstractValidator<AddCustomerInformationCommand>
{
    public AddCustomerInformationCommandValidation()
    {
        RuleFor(c => c.FullName)
            .MinimumLength(FullName.MinLength)
            .MaximumLength(FullName.MaxLength)
            .NotEmpty()
            .NotNull();
    }
}
public sealed class AddCustomerInformationCommandHandler
(IOrderRepository orderRepository)
    :ICommandHandler<AddCustomerInformationCommand>
{
    public async Task<Result> Handle(AddCustomerInformationCommand request, CancellationToken cancellationToken)
    {
        var orderIdResult = OrderId.CreateFrom(request.OrderId);
        if (orderIdResult.IsFailure)
            return orderIdResult.Error;
        Order? order =await orderRepository.FindAsync(orderIdResult.Value,cancellationToken);
        if (order is null)
            return Error.NotFound("AddCustomerInformationCommand.OrderNotFound", "سفارش یافت نشد.");

        Result orderResult = order.UpdateCustomerInformation(FullName.Create(request.FullName).Value);
        return orderResult;
    }
}
