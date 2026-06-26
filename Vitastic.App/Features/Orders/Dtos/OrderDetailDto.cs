namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// Order Detail DTO
/// </summary>
public sealed record OrderDetailDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; }=string.Empty;
    public Guid UserId { get; init; }
    public string UserFullName { get; init; }=string.Empty;
    public string UserEmail { get; init; }=string.Empty;
    public decimal ItemsTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal FinalAmount { get; init; }
    public OrderStatusDto Status { get; init; }
    public string PaymentMethod { get; init; }=string.Empty;
    public string? PaymentGateway { get; init; }
    public DateTimeOffset?  PaymentDate { get; init; }
    public string? DiscountCode { get; init; }
    public string? PhoneNumber { get; init; }
    public string? BillingAddress { get; init; }
    public string? ShippingAddress { get; init; }
    public string? CustomerNote { get; init; }
    public string? AdminNote { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset?  UpdatedAt { get; init; }
    public DateTimeOffset?  CompletedAt { get; init; }
    public DateTimeOffset?  CancelledAt { get; init; }

    public OrderDetailDto() { }
}
