namespace Vitastic.App.Features.Orders.Dtos;

/// <summary>
/// Order Item DTO
/// For displaying details of each item in an order
/// </summary>
public sealed record OrderItemDto
{
    public OrderItemDto() { }
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; }=string.Empty;
    public string ThumbnailUrl { get; init; }=string.Empty;
    public string InstructorFullName { get; init; }=string.Empty;
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
