using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Data;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.ChangeStatus;

public record ChangeOrderStatusCommand(
    Guid OrderId,
    OrderStatusDto Status,
    string? AdminNote
) : ICommand;

public sealed class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
{
    public ChangeOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("شناسه سفارش الزامی است");

        RuleFor(x => x.Status)
            .Must(Enum.IsDefined)
            .WithMessage(x => $"وضعیت سفارش نامعتبر است: {x.Status} ({(int)x.Status})");


        RuleFor(x => x.AdminNote)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.AdminNote))
            .WithMessage("یادداشت مدیر نمی‌تواند بیشتر از 1000 کاراکتر باشد");
    }
}

public sealed class ChangeOrderStatusCommandHandler(
    IOrderRepository orderRepository)
    : ICommandHandler<ChangeOrderStatusCommand>
{
    public async Task<Result> Handle(
        ChangeOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var orderIdResult = OrderId.CreateFrom(request.OrderId);
        if (orderIdResult.IsFailure)
            return orderIdResult.Error;
        Order? order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);

        if (order is null)
            return Error.NotFound("ChangeOrderStatusCommand.OrderNotFound", "سفارش یافت نشد.");

        var newStatus = MapToDomainStatus(request.Status);
        // Use domain method to change status (encapsulates business rules)
        var result = order.ChangeStatus(newStatus, request.AdminNote);
        await orderRepository.UpdateAsync(order, cancellationToken);
        return result.IsFailure ? result : Result.Success();
    }

    private static OrderStatus MapToDomainStatus(OrderStatusDto dto)
    {
        return dto switch
        {
            OrderStatusDto.Pending => OrderStatus.Pending,
            OrderStatusDto.Processing => OrderStatus.Processing,
            OrderStatusDto.Completed => OrderStatus.Completed,
            OrderStatusDto.Cancelled => OrderStatus.Cancelled,
            OrderStatusDto.Refunded => OrderStatus.Refunded,
            OrderStatusDto.Failed => OrderStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(dto))
        };
    }
}
