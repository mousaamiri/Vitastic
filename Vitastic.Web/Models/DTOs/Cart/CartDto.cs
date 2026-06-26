namespace Vitastic.Web.Models.DTOs.Cart;

#region CartItemDto

public sealed record CartItemDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string CourseImageName,
    decimal UnitPrice,
    string Currency,
    string CourseInstructorName,
    DateTimeOffset CreatedAt
);

#endregion

#region CartDto

public sealed record CartDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public string? UserFullName { get; init; }
    public List<CartItemDto> Items { get; init; } = [];
    public decimal ItemsTotal { get; init; }
    public string Currency { get; init; } = "تومان";
    public int ItemsCount { get; init; }
    public DateTimeOffset LastModifiedAt { get; init; }
}

#endregion

#region CartSummaryDto

public sealed record CartSummaryDto
{
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal VatAmount { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "تومان";
    public string? AppliedCouponCode { get; init; }
    public bool HasDiscount => DiscountAmount > 0;
    public bool IsFreeShipping => ShippingCost == 0;
}

#endregion

#region AddCartItemRequest

public sealed record AddCartItemRequest(Guid CourseId);

#endregion

#region ApplyCouponRequest

public sealed record ApplyCouponRequest(string CouponCode);

#endregion
