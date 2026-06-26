using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class OrderRepository(ApplicationWriteDbContext context) :
    BaseRepository<Order, OrderId>(context), IOrderRepository
{
    public new async Task<Order?> FindAsync(OrderId id, CancellationToken cancellation = default)
        =>await ExecuteAsync(
            async () =>await Context.Orders.Include(o=>o.Items)
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellation),
            cancellation);
    public async Task<Order?> GetOpenOrderAsync(OrderId orderId, CancellationToken cancellation = default)
    {
        return await ExecuteAsync(() =>

           Context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellation)
        ,cancellation);
    }

    public async Task<Order?> GetOpenOrderByUserIdAsync(UserId userId, CancellationToken cancellation = default)
    {
        return await ExecuteAsync(() =>
                 Context.Orders
                    .Include(o => o.Items)
                    .Where(o => o.UserId == userId && o.Status == OrderStatus.Pending)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync(cancellation)
            ,cancellation);    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(UserId userId, CancellationToken cancellation = default)
    {
        return await ExecuteAsync(() =>
                Context.Orders
                    .Include(o => o.Items)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync(cancellation)
            ,cancellation);    }

    public async Task<bool> ItemIsPurchasedAsync(CourseId courseId, UserId userId, CancellationToken token = default)
        => await ExecuteAsync(async () =>
        {
            var userHasOrder = await Context.Orders.AnyAsync(x => x.UserId == userId, token);
            if(!userHasOrder) return false;
            List<OrderId> userOrderIds = await Context.Orders.Where(o => o.UserId == userId).Select(o => o.Id).ToListAsync(token);
           return await Context.OrderItems.AnyAsync(x => x.CourseId == courseId && userOrderIds.Contains(x.OrderId), token);
        }, token);

    public async Task<Order?> GetPendingOrderByUserIdAsync(UserId userId, CancellationToken ct = default)
        => await ExecuteAsync(async ()=>
            await Context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(ct), ct);

    public async Task<bool> CheckIsPaidableAsync(OrderId orderId, CancellationToken token = default)
        => await ExecuteAsync(async () =>
        {
            var order = await Context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new { o.Status })
                .FirstOrDefaultAsync(token);  // ← Safe: returns null if not found

            if (order is null)
                return false;

            return order.Status is OrderStatus.Pending or OrderStatus.Processing;
        }, token);
}
