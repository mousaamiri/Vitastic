using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

/// <summary>
/// Query Service Interface for Order Entity
/// For READ operations only
/// </summary>
public interface IOrderQueryService
{
    // ==================== LIST ====================
    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        OrderStatusDto? status = null,
        CancellationToken cancellationToken = default);

    // ==================== DETAIL ====================
     Task<OrderDetailDto?> GetByIdAsync(
         OrderId orderId,
        CancellationToken cancellationToken = default);

    // ==================== USER ORDERS ====================
    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetByUserIdAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        OrderStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<OrderDto?> GetLatestOrderByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    // ==================== SEARCH ====================
    Task<(IReadOnlyList<OrderDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        OrderStatusDto? status = null,
        CancellationToken cancellationToken = default);


    // ==================== PAYMENT ====================
    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPaidOrdersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPaidOrdersByIdAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPendingOrdersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OrderDto> Items, int Total)> GetPendingOrdersByIdAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    // ==================== ITEMS ====================
    /// <summary>
    /// Get order items for a specific order
    /// </summary>
    Task<IReadOnlyList<OrderItemDto>> GetOrderItemsAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has access to a course through any of their orders
    /// </summary>
    Task<bool> UserHasCourseAccessAsync(
        UserId userId,
        CourseId courseId,
        CancellationToken cancellationToken = default);

    // ==================== STATISTICS ====================
    /// <summary>
    /// Get overall order statistics (total orders, total revenue, etc.)
    /// </summary>
    Task<OrderStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get order statistics for a specific user (total orders, total spent, etc.)
    /// </summary>
    Task<UserOrderStatisticsDto> GetUserStatisticsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get statistics for orders that were completed in the given time period
    /// </summary>
    Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(
        DateTimeOffset?  fromDate = null,
        DateTimeOffset?  toDate = null,
        CancellationToken cancellationToken = default);

    // ==================== UTILITIES ====================
    /// <summary>
    /// Check if an order number already exists (for validation purposes)
    /// </summary>
    Task<bool> IsOrderNumberExistAsync(
        string orderNumber,
        OrderId? excludeOrderId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the total number of orders for a specific user
    /// </summary>
    Task<int> GetUserOrdersCountAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
