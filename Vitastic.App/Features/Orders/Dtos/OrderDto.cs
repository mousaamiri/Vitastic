namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// Order List DTO
/// For displaying list of orders
/// </summary>
public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    string UserFullName,
    string UserEmail,
    decimal ItemsTotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal ShippingAmount,
    decimal FinalAmount,
    string Status,
    string PaymentMethod,
    int ItemsCount,
    bool IsPaid,
    DateTimeOffset CreatedAt,
    DateTimeOffset?  CompletedAt);
