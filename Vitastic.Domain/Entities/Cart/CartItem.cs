using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Cart;

public class CartItem : FullEntity<CartItemId>
{
    #region Properties

    public CartId CartId { get; private set; }
    public CourseId CourseId { get; private set; }

    // Snapshot — can be refreshed unlike OrderItem
    public CourseTitle CourseTitle { get; private set; }
    public FullName CourseInstructorName { get; private set; }
    public CourseImageName? CourseImageName { get; private set; }

    // Snapshot of price at the time of adding to cart
    public Money UnitPrice { get; private set; }

    #endregion

    #region Constructors

    // EF Constructor
    private CartItem() { }

    // Private Constructor — accepts only Value Objects
    private CartItem(
        CartItemId id,
        CartId cartId,
        CourseId courseId,
        CourseTitle courseTitle,
        FullName instructorFullName,
        CourseImageName? courseImageName,
        Money unitPrice) : base(id)
    {
        CartId = cartId;
        CourseId = courseId;
        CourseTitle = courseTitle;
        CourseInstructorName = instructorFullName;
        CourseImageName = courseImageName;
        UnitPrice = Money.Create(unitPrice.Value, unitPrice.Currency).Value;
    }

    #endregion

    #region Factory Method

    /// <summary>
    /// Creates a CartItem from value objects only — no Course entity reference
    /// </summary>
    public static Result<CartItem> Create(
        CartId cartId,
        CourseId courseId,
        CourseTitle courseTitle,
        FullName instructorFullName,
        CourseImageName? courseImageName,
        Money unitPrice)
    {
        if (cartId is null)
            return CartItemErrors.InvalidCartId;

        if (courseId is null)
            return CartItemErrors.InvalidCourse;

        if (courseTitle is null)
            return CartItemErrors.InvalidCourse;

        if (instructorFullName is null)
            return CartItemErrors.InvalidInstructor;

        if (unitPrice is null || unitPrice.Value < 0)
            return CartItemErrors.InvalidPrice;

        var item = new CartItem(
            CartItemId.New(),
            cartId,
            courseId,
            courseTitle,
            instructorFullName,
            courseImageName,
            unitPrice
        );

        return Result.Success(item);
    }

    #endregion

    #region Behaviors

    /// <summary>
    /// Refresh price snapshot from current course price — Cart allows this, Order does not
    /// </summary>
    public Result RefreshPrice(Money newPrice)
    {
        if (newPrice is null || newPrice.Value < 0)
            return CartItemErrors.InvalidPrice;

        if (!newPrice.Currency.Equals(UnitPrice.Currency))
            return CartItemErrors.CurrencyMismatch;

        UnitPrice = Money.Create(newPrice.Value, newPrice.Currency).Value;
        return Result.Success();
    }

    /// <summary>
    /// Update course snapshot details (title, image, instructor)
    /// </summary>
    public Result UpdateSnapshot(
        CourseTitle? title = null,
        CourseImageName? imageName = null,
        FullName? instructorName = null)
    {
        if (title is not null) CourseTitle = title;
        if (imageName is not null) CourseImageName = imageName;
        if (instructorName is not null) CourseInstructorName = instructorName;

        return Result.Success();
    }

    #endregion
}

public static class CartItemErrors
{
    #region Validation

    public static readonly Error InvalidCartId =
        Error.Validation("CartItem.InvalidCartId", "شناسه سبد خرید نامعتبر است.");

    public static readonly Error InvalidCourse =
        Error.Validation("CartItem.InvalidCourse", "دوره نامعتبر است.");

    public static readonly Error InvalidInstructor =
        Error.Validation("CartItem.InvalidInstructor", "اطلاعات مدرس نامعتبر است.");

    public static readonly Error InvalidPrice =
        Error.Validation("CartItem.InvalidPrice", "قیمت دوره نامعتبر است.");

    public static readonly Error NegativePrice =
        Error.Validation("CartItem.NegativePrice", "قیمت نمی‌تواند منفی باشد.");

    public static readonly Error CurrencyMismatch =
        Error.Validation("CartItem.CurrencyMismatch", "واحد پول نامعتبر است.");

    #endregion

    #region Business

    public static readonly Error CourseMismatch =
        Error.Validation("CartItem.CourseMismatch", "دوره ارسال شده با آیتم سبد خرید مطابقت ندارد.");

    #endregion
}
