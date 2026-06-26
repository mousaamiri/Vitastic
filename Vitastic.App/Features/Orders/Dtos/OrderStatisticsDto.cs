namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// Order Statistics DTO
/// For displaying overall order statistics
/// </summary>
public sealed record OrderStatisticsDto(
    int TotalOrders,
    int PendingOrders,
    int ProcessingOrders,
    int CompletedOrders,
    int CancelledOrders,
    int RefundedOrders,
    decimal TotalRevenue,
    decimal AverageOrderValue);
