namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// Revenue Statistics DTO
/// For displaying revenue statistics for a given time period
/// </summary>
public sealed record RevenueStatisticsDto(
    int TotalCompletedOrders,
    decimal TotalRevenue,
    decimal AverageOrderValue,
    decimal HighestOrderValue,
    decimal LowestOrderValue,
    int UniqueCustomers,
    decimal TotalDiscount);
