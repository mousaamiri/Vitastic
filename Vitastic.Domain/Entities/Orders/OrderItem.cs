using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Orders;

public class OrderItem : FullEntity<OrderItemId>
{
    #region Properties

    public OrderId OrderId { get; private set; }

    // Only store the ID as a reference — not the entity itself
    public CourseId CourseId { get; private set; }

    // Snapshot data — frozen at the time of purchase
    public CourseTitle CourseTitle { get; private set; }
    public CourseImageName? CourseImageName { get; private set; }
    public FullName InstructorFullName { get; private set; }

    // Financial snapshot
    public Money UnitPrice { get; init; }
    public Money DiscountAmount { get; private set; } = Money.Zero();
    public Money FinalPrice { get; private set; }

    // Access management
    public DateTimeOffset? AccessExpiryDate { get; private set; }
    public bool IsAccessGranted { get; private set; }
    public DateTimeOffset? AccessGrantedAt { get; private set; }
    public DateTimeOffset? AccessRevokedAt { get; private set; }

    #endregion

    #region Constructors

    // EF Constructor
    private OrderItem() { }

    // Private Constructor — accepts only Value Objects, no entities
    private OrderItem(
        OrderItemId id,
        OrderId orderId,
        CourseId courseId,
        CourseTitle courseTitle,
        CourseImageName? courseImageName,
        FullName instructorFullName,
        Money unitPrice) : base(id)
    {
        OrderId = orderId;
        CourseId = courseId;

        // Snapshot — these values are frozen at order time
        CourseTitle = courseTitle;
        CourseImageName = courseImageName;
        InstructorFullName = instructorFullName;

        // Create separate Money instances for EF Core change tracking
        UnitPrice = Money.Create(unitPrice.Value, unitPrice.Currency).Value;
        DiscountAmount = Money.Create(0, unitPrice.Currency).Value;
        FinalPrice = Money.Create(unitPrice.Value, unitPrice.Currency).Value;

        IsAccessGranted = false;
    }

    #endregion

    #region Factory Method

    /// <summary>
    /// Creates an OrderItem from primitive/value-object data only.
    /// No entity references — respects Aggregate boundaries.
    /// </summary>
    public static Result<OrderItem> Create(
        OrderId orderId,
        CourseId courseId,
        CourseTitle courseTitle,
        CourseImageName? courseImageName,
        FullName instructorFullName,
        Money unitPrice)
    {
        if (orderId is null)
            return OrderItemErrors.InvalidOrder;

        if (courseId is null)
            return OrderItemErrors.InvalidCourse;

        if (courseTitle is null)
            return OrderItemErrors.InvalidCourseTitle;

        if (instructorFullName is null)
            return OrderItemErrors.InvalidInstructor;

        if (unitPrice is null || unitPrice.Value < 0)
            return OrderItemErrors.InvalidPrice;

        var item = new OrderItem(
            OrderItemId.New(),
            orderId,
            courseId,
            courseTitle,
            courseImageName,
            instructorFullName,
            unitPrice
        );

        return Result.Success(item);
    }

    #endregion

    #region Discount Management

    /// <summary>
    /// Apply discount for order item
    /// </summary>
    public Result ApplyDiscount(Money discountAmount)
    {
        if (discountAmount is null)
            return OrderItemErrors.InvalidDiscount;

        if (discountAmount.Value <= 0)
            return OrderItemErrors.DiscountMustBePositive;

        if (discountAmount.Value > UnitPrice.Value)
            return OrderItemErrors.DiscountExceedsPrice;

        if (!discountAmount.Currency.Equals(UnitPrice.Currency))
            return OrderItemErrors.CurrencyMismatch;

        Result<Money> subtractResult = UnitPrice.Subtract(discountAmount);
        if (subtractResult.IsFailure)
            return subtractResult.Error;

        DiscountAmount = discountAmount;
        FinalPrice = subtractResult.Value;

        return Result.Success();
    }

    /// <summary>
    /// Remove discount from order item
    /// </summary>
    public Result RemoveDiscount()
    {
        if (DiscountAmount.Value == 0)
            return Result.Success();

        DiscountAmount = Money.Create(0, UnitPrice.Currency).Value;
        FinalPrice = UnitPrice;

        return Result.Success();
    }

    #endregion

    #region Access Management

    /// <summary>
    /// Grant access to the course
    /// </summary>
    public Result GrantAccess(DateTimeOffset? expiryDate = null)
    {
        if (IsAccessGranted)
            return OrderItemErrors.AccessAlreadyGranted;

        if (expiryDate.HasValue && expiryDate.Value <= DateTimeOffset.UtcNow)
            return OrderItemErrors.ExpiryDateMustBeInFuture;

        IsAccessGranted = true;
        AccessGrantedAt = DateTimeOffset.UtcNow;
        AccessExpiryDate = expiryDate;
        AccessRevokedAt = null;

        return Result.Success();
    }

    /// <summary>
    /// Revoke access from course
    /// </summary>
    public Result RevokeAccess()
    {
        if (!IsAccessGranted)
            return OrderItemErrors.AccessNotGranted;

        IsAccessGranted = false;
        AccessRevokedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Extend access period
    /// </summary>
    public Result ExtendAccess(DateTimeOffset newExpiryDate)
    {
        if (!IsAccessGranted)
            return OrderItemErrors.AccessNotGranted;

        if (newExpiryDate <= DateTimeOffset.UtcNow)
            return OrderItemErrors.ExpiryDateMustBeInFuture;

        if (AccessExpiryDate.HasValue && newExpiryDate <= AccessExpiryDate.Value)
            return OrderItemErrors.NewExpiryMustBeLater;

        AccessExpiryDate = newExpiryDate;

        return Result.Success();
    }

    /// <summary>
    /// Check if access is currently valid
    /// </summary>
    public bool HasValidAccess()
    {
        if (!IsAccessGranted)
            return false;
        if (!AccessExpiryDate.HasValue)
            return true;

        return AccessExpiryDate.Value >= DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Time remaining until expiration
    /// </summary>
    public TimeSpan? GetRemainingAccessTime()
    {
        if (!IsAccessGranted || !AccessExpiryDate.HasValue)
            return null;

        TimeSpan remaining = AccessExpiryDate.Value - DateTimeOffset.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    #endregion

    #region Queries

    public decimal GetDiscountPercentage()
    {
        if (UnitPrice.Value == 0) return 0;
        return (DiscountAmount.Value / UnitPrice.Value) * 100;
    }

    public Money GetSavedAmount() => DiscountAmount;

    public bool HasDiscount() => DiscountAmount.Value > 0;

    #endregion
}
