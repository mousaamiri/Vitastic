namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// User Order Statistics DTO
/// For displaying order statistics for a specific user
/// </summary>
public sealed record UserOrderStatisticsDto(
    int TotalOrders,
    int CompletedOrders,
    decimal TotalSpent,
    int TotalCoursesEnrolled,
    DateTimeOffset? FirstOrderDate,
    DateTimeOffset? LastOrderDate);
