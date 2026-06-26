namespace Vitastic.Api.Features.Orders.Responses;

public sealed record OrderDetailResponse
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserFullName { get; init; }=string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public decimal ItemsTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal FinalAmount { get; init; }
    public OrderStatusResponse Status { get; init; }
    public string PaymentMethod { get; init; }=string.Empty;
    public string? PaymentGateway { get; init; }
    public DateTimeOffset?  PaymentDate { get; init; }
    public string? DiscountCode { get; init; }
    public string? PhoneNumber { get; init; }
    public string? BillingAddress { get; init; }
    public string? ShippingAddress { get; init; }
    public string? CustomerNote { get; init; }
    public string? AdminNote { get; init; }
    public List<OrderItemResponse> Items { get; init; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset?  UpdatedAt { get; init; }
    public DateTimeOffset?  CompletedAt { get; init; }
    public DateTimeOffset?  CancelledAt { get; init; }

    public OrderDetailResponse() { }
}

public sealed record OrderResponse(
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

public sealed record OrderItemResponse
{
    public OrderItemResponse() { }
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; }=string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? InstructorFullName { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal FinalPrice { get; init; }
    public bool HasDiscount { get; init; }
    public decimal DiscountPercentage { get; init; }
    public bool IsAccessGranted { get; init; }
    public DateTimeOffset?  AccessGrantedAt { get; init; }
    public DateTimeOffset?  AccessExpiryDate { get; init; }
    public DateTimeOffset?  AccessRevokedAt { get; init; }
}
public sealed record OrderStatisticsResponse(
    int TotalOrders,
    int PendingOrders,
    int ProcessingOrders,
    int CompletedOrders,
    int CancelledOrders,
    int RefundedOrders,
    decimal TotalRevenue,
    decimal AverageOrderValue);
public enum OrderStatusResponse
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4,
    Refunded = 5,
    Failed = 6

}
public enum PaymentMethodResponse
{
    Online = 1,
    Wallet = 2
}
public sealed record RevenueStatisticsResponse(
    int TotalCompletedOrders,
    decimal TotalRevenue,
    decimal AverageOrderValue,
    decimal HighestOrderValue,
    decimal LowestOrderValue,
    int UniqueCustomers,
    decimal TotalDiscount);
public sealed record UserOrderStatisticsResponse(
    int TotalOrders,
    int CompletedOrders,
    decimal TotalSpent,
    int TotalCoursesEnrolled,
    DateTimeOffset? FirstOrderDate,
    DateTimeOffset? LastOrderDate);
