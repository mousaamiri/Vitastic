namespace Vitastic.Web.Models.DTOs.Order;

public record OrderDetailApiDto(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    string UserFullName,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string UserEmail,
    decimal ItemsTotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal ShippingAmount,
    decimal FinalAmount,
    OrderStatusDto Status,
    string PaymentMethod,
    string? PaymentGateway,
    DateTimeOffset? PaymentDate,
    string? DiscountCode,
    string? BillingAddress,
    string? ShippingAddress,
    string? CustomerNote,
    string? AdminNote,
    List<OrderItemApiDto> Items,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? CancelledAt
);
public record OrderItemApiDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string? ThumbnailUrl,
    string? InstructorFullName,
    decimal UnitPrice,
    decimal DiscountAmount,
    decimal FinalPrice,
    bool HasDiscount,
    decimal DiscountPercentage,
    bool IsAccessGranted,
    DateTimeOffset? AccessGrantedAt,
    DateTimeOffset? AccessExpiryDate,
    DateTimeOffset? AccessRevokedAt
);

public enum OrderStatusDto
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4,
    Refunded = 5,
    Failed = 6
}
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
    DateTimeOffset? CompletedAt);
public class PaymentMethodOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
public sealed record ChangeOrderStatusByAdminRequest(OrderStatusDto Status, string? AdminNote);
