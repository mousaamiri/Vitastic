using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

/// <summary>
/// Order Query Service - Read operations only
/// Uses LINQ for simple queries and Dapper/Raw SQL for complex ones
/// </summary>
internal class OrderQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<OrderQueryService> logger) : IOrderQueryService
{
    // ==================== LIST (PAGINATION) ====================

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        OrderStatusDto? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Order> query = readDbContext.Orders
                .Include(o => o.Items)
                .AsQueryable();

            if (status.HasValue)
            {
                var state = (OrderStatus)status.Value;
                query = query.Where(o => o.Status.Equals(state));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<OrderDto>>(items);

            logger.LogInformation(
                "Listed {Count} orders - Page {Page}/{Total}, Status: {Status}",
                dtos.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize,
                status?.ToString() ?? "All");

            return (dtos, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    // ==================== DETAIL ====================

    public async Task<OrderDetailDto?> GetByIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Order? order = await readDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is null)
            {
                logger.LogWarning("Order not found: {OrderId}", orderId);
                return null;
            }

            var detail = mapper.Map<OrderDetailDto>(order);

            logger.LogInformation("Order detail retrieved: {OrderId}", orderId);

            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByIdAsync");
            throw;
        }
    }

    // ==================== USER ORDERS ====================

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> GetByUserIdAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        OrderStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Order> query = readDbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<OrderDto>>(items);

            logger.LogInformation(
                "Retrieved {Count} orders for user {UserId} - Page {Page}/{Total}",
                dtos.Count,
                userId,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (dtos, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByUserIdAsync");
            throw;
        }
    }

    public async Task<OrderDto?> GetLatestOrderByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await readDbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
            {
                logger.LogWarning("No orders found for user: {UserId}", userId);
                return null;
            }

            var dto = mapper.Map<OrderDto>(order);

            logger.LogInformation("Latest order retrieved for user: {UserId}", userId);

            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetLatestOrderByUserIdAsync");
            throw;
        }
    }

    // ==================== SEARCH ====================

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        OrderStatusDto? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            logger.LogInformation(
                "Searching orders - Term: {SearchTerm}, Status: {Status}",
                searchTerm,
                status?.ToString() ?? "All");

            var term = searchTerm.ToLower().Trim();
            IQueryable<Order> query = readDbContext.Orders
                .Include(o => o.Items);

            if (status.HasValue)
            {
                var state = (OrderStatus)status.Value;
                query = query.Where(o => o.Status.Equals(state));
            }
            var total = await query.CountAsync(cancellationToken);

            if (total == 0)
            {
                logger.LogInformation(
                    "No orders found for search term: {SearchTerm}",
                    searchTerm);
                return (new List<OrderDto>(), 0);
            }

            var items = (await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken))
                .Where(o =>
                    o.OrderNumber.ToLower().Contains(term)||
                    o.UserFullName.ToLower().Contains(term) ||
                    o.UserEmail.ToLower().Contains(term))
                .ToList();


            logger.LogInformation(
                "Search completed - Term: {SearchTerm}, Results: {Count}, Page: {Page}/{Total}",
                searchTerm,
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Search operation cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SearchAsync");
            throw;
        }
    }

    // ==================== STATUS ====================

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> GetByStatusAsync(
        OrderStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            var query = readDbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            List<OrderDto> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} orders with status {Status} - Page {Page}/{Total}",
                items.Count,
                status,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByStatusAsync");
            throw;
        }
    }

    // ==================== PAYMENT ====================

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPaidOrdersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            var query = readDbContext.Orders
                .Include(o => o.Items)
                .Where(o => (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Refunded)
                    && o.PaymentDate.HasValue)
                .OrderByDescending(o => o.PaymentDate);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)

                .ToListAsync(cancellationToken);


            logger.LogInformation(
                "Retrieved {Count} paid orders - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPaidOrdersAsync");
            throw;
        }
    }

    public Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPaidOrdersByIdAsync(UserId userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPendingOrdersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            var query = readDbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing)
                .OrderByDescending(o => o.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)

                .ToListAsync(cancellationToken);


            logger.LogInformation(
                "Retrieved {Count} pending orders - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPendingOrdersAsync");
            throw;
        }
    }

    public Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPendingOrdersByIdAsync(UserId userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // ==================== ITEMS ====================

    public async Task<IReadOnlyList<OrderItemDto>> GetOrderItemsAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
           List<OrderItem> items = await readDbContext.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync(cancellationToken);

            List<OrderItemDto> dtos = mapper.Map<List<OrderItemDto>>(items);
            logger.LogInformation(
                "Retrieved {Count} items for order: {OrderId}",
                items.Count,
                orderId);

            return dtos;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetOrderItemsAsync");
            throw;
        }
    }


    public async Task<bool> UserHasCourseAccessAsync(
        UserId userId, CourseId courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            List<OrderId> userOrderIds = await readDbContext.Orders
                .Where(ou => ou.UserId == userId)
                .Distinct()
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);
            var hasAccess = await readDbContext.OrderItems
                .AnyAsync(oi =>
                    userOrderIds.Contains(oi.OrderId) &&
                    oi.CourseId == courseId &&
                    oi.IsAccessGranted,
                    cancellationToken);

            logger.LogInformation(
                "User course access check - User: {UserId}, Course: {CourseId}, HasAccess: {HasAccess}",
                userId,
                courseId,
                hasAccess);

            return hasAccess;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in UserHasCourseAccessAsync");
            throw;
        }
    }

    // ==================== STATISTICS ====================

    public async Task<OrderStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT
                    COUNT(*) as TotalOrders,
                    COUNT(CASE WHEN Status = 0 THEN 1 END) as PendingOrders,
                    COUNT(CASE WHEN Status = 1 THEN 1 END) as ProcessingOrders,
                    COUNT(CASE WHEN Status = 2 THEN 1 END) as CompletedOrders,
                    COUNT(CASE WHEN Status = 3 THEN 1 END) as CancelledOrders,
                    COUNT(CASE WHEN Status = 4 THEN 1 END) as RefundedOrders,
                    COALESCE(SUM(CASE WHEN Status = 2 THEN FinalAmount ELSE 0 END), 0) as TotalRevenue,
                    COALESCE(AVG(CASE WHEN Status = 2 THEN FinalAmount ELSE NULL END), 0) as AverageOrderValue
                FROM Orders";

            var result = await connection.QuerySingleAsync<OrderStatisticsDto>(sql);

            logger.LogInformation("Order statistics retrieved");

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetStatisticsAsync");
            throw;
        }
    }

    public async Task<UserOrderStatisticsDto> GetUserStatisticsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await readDbContext.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync(cancellationToken);

            var completedOrders = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
            List<OrderId> userOrderIds = await readDbContext.Orders
                .Where(ou => ou.UserId == userId)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);
            var dto = new UserOrderStatisticsDto(
                TotalOrders: orders.Count,
                CompletedOrders: completedOrders.Count,
                TotalSpent: completedOrders.Any()
                    ? completedOrders.Sum(o => o.FinalAmount.Value)
                    : 0m,
                TotalCoursesEnrolled: await readDbContext.OrderItems
                    .Where(oi => userOrderIds.Contains(oi.OrderId) && oi.IsAccessGranted)
                    .CountAsync(cancellationToken),
                FirstOrderDate: orders.Any() ? orders.Min(o => o.CreatedAt) : null,
                LastOrderDate: orders.Any() ? orders.Max(o => o.CreatedAt) : null);

            logger.LogInformation("User statistics retrieved for user: {UserId}", userId);

            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetUserStatisticsAsync");
            throw;
        }
    }

    public async Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(
        DateTimeOffset?  fromDate = null,
        DateTimeOffset?  toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var fromDateParam = fromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDateParam = toDate ?? DateTime.UtcNow;

            const string sql = @"
                SELECT
                    COUNT(*) as TotalCompletedOrders,
                    COALESCE(SUM(FinalAmount), 0) as TotalRevenue,
                    COALESCE(AVG(FinalAmount), 0) as AverageOrderValue,
                    COALESCE(MAX(FinalAmount), 0) as HighestOrderValue,
                    COALESCE(MIN(FinalAmount), 0) as LowestOrderValue,
                    COUNT(DISTINCT UserId) as UniqueCustomers,
                    COALESCE(SUM(DiscountAmount), 0) as TotalDiscount
                FROM Orders
                WHERE Status = 2 AND CompletedAt BETWEEN @FromDate AND @ToDate";

            var parameters = new { FromDate = fromDateParam, ToDate = toDateParam };

            var result = await connection.QuerySingleAsync<RevenueStatisticsDto>(sql, parameters);

            logger.LogInformation(
                "Revenue statistics retrieved - From: {FromDate}, To: {ToDate}",
                fromDateParam,
                toDateParam);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetRevenueStatisticsAsync");
            throw;
        }
    }

    public Task<bool> IsOrderNumberExistAsync(string orderNumber, OrderId? excludeOrderId = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // ==================== UTILITIES ====================

    public async Task<bool> IsOrderNumberExistAsync(
        string orderNumber,
        Guid? excludeOrderId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderNumber);

            var query = readDbContext.Orders
                .Where(o => o.OrderNumber.Value == orderNumber);

            if (excludeOrderId.HasValue)
                query = query.Where(o => o.Id.Value != excludeOrderId.Value);

            var exists = await query.AnyAsync(cancellationToken);

            logger.LogInformation(
                "Order number existence check: {OrderNumber}, Exists: {Exists}",
                orderNumber,
                exists);

            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in IsOrderNumberExistAsync");
            throw;
        }
    }

    public async Task<int> GetUserOrdersCountAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await readDbContext.Orders
                .Where(o => o.UserId == userId)
                .CountAsync(cancellationToken);

            logger.LogInformation("User orders count: {UserId}, Count: {Count}", userId, count);

            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetUserOrdersCountAsync");
            throw;
        }
    }
}
