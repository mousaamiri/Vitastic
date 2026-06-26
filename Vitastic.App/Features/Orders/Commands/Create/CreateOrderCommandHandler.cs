using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.Create
{
    public sealed class CreateOrderCommandHandler(
        IUserRepository userRepository,
        IOrderRepository orderRepository) : ICommandHandler<CreateOrderCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            User? user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
            if (user == null)
                return Error.NotFound("CreateOrderCommand.UserNotFound", "کاربر یافت نشد.");
            Order? openOrder = await orderRepository.GetOpenOrderByUserIdAsync(userIdResult.Value,cancellationToken);
            if (openOrder is not null)
                return openOrder.Id.Value;
            Result<Order> orderResult = Order.Create(user.Id,
                user.UserFullName,
                user.Email,
                user.PhoneNumber);

            if (orderResult.IsFailure)
                return orderResult.Error;

            Order order = orderResult.Value;

            await orderRepository.AddAsync(order, cancellationToken);

            return order.Id.Value;
        }
    }
}
