using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Shared.Repositories;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    /// <summary>
    /// Receive pending order from user with specified ID
    /// </summary>
    Task<Order?> GetOpenOrderAsync(
        OrderId orderId,
        CancellationToken cancellation = default);

    Task<Order?> GetOpenOrderByUserIdAsync(
        UserId userId,
        CancellationToken cancellation = default);

    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(
        UserId userId,
        CancellationToken cancellation = default);

    Task<bool> ItemIsPurchasedAsync(CourseId courseId, UserId userId, CancellationToken token=default);
    Task<Order?> GetPendingOrderByUserIdAsync(UserId userId, CancellationToken ct = default);
    Task<bool> CheckIsPaidableAsync(OrderId orderId, CancellationToken token = default);
}
