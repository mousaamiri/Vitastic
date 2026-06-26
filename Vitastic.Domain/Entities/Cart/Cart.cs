#region Using

using Vitastic.Domain.Entities.Cart.Events;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

#endregion

namespace Vitastic.Domain.Entities.Cart;

public class Cart : AggregateRoot<CartId>
{
    #region Properties

    public UserId? UserId { get; private set; }

    // Guest session identifier
    public string? SessionId { get; private set; }

    public FullName? UserFullName { get; private set; }

    public Email? UserEmail { get; private set; }

    public bool IsGuest => UserId is null;

    public Money ItemsTotal => CalculateTotal();

    public int ItemsCount => _items.Count;

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private readonly List<CartItem> _items = [];

    #endregion

    #region Constructors

    protected Cart() { } // EF Core

    private Cart(
        CartId id,
        UserId? userId,
        string? sessionId,
        FullName? userFullName,
        Email? userEmail) : base(id)
    {
        UserId = userId;
        SessionId = sessionId;
        UserFullName = userFullName;
        UserEmail = userEmail;

        MarkAsModified();
    }

    #endregion

    #region Factory

    /// <summary>
    /// Creates a cart for an authenticated user using only Value Objects.
    /// Caller (Application Layer) extracts these from the User aggregate.
    /// </summary>
    public static Result<Cart> Create(
        UserId userId,
        FullName userFullName,
        Email userEmail)
    {
        if (userId is null)
            return CartErrors.InvalidUser;

        if (userFullName is null)
            return CartErrors.InvalidUserName;

        if (userEmail is null)
            return CartErrors.InvalidUserEmail;

        return Result.Success(new Cart(
            CartId.New(),
            userId,
            null,
            userFullName,
            userEmail));
    }

    /// <summary>
    /// Creates a cart for a guest user identified by session ID.
    /// </summary>
    public static Result<Cart> CreateForGuest(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return CartErrors.InvalidGuestId;

        return Result.Success(new Cart(
            CartId.New(),
            null,
            sessionId,
            null,
            null));
    }

    #endregion

    #region Cart Management

    /// <summary>
    /// Adds a course item to the cart using Value Objects only.
    /// </summary>
    public Result<CartItem> AddItem(
        CourseId courseId,
        CourseTitle courseTitle,
        FullName instructorName,
        CourseImageName imageName,
        Money price)
    {
        if (_items.Any(i => i.CourseId.Equals(courseId)))
            return CartErrors.CourseAlreadyInCart;

        // Currency consistency check
        if (_items.Count > 0)
        {
            var currency = _items.First().UnitPrice.Currency;

            if (!price.Currency.Equals(currency))
                return CartErrors.CurrencyMismatch;
        }

        var itemResult = CartItem.Create(
            Id,
            courseId,
            courseTitle,
            instructorName,
            imageName,
            price);

        if (itemResult.IsFailure)
            return itemResult.Error;

        _items.Add(itemResult.Value);

        Touch();

        RaiseDomainEvent(
            CartItemAddedDomainEvent.Create(
                Id,
                itemResult.Value.Id,
                courseId));

        return Result.Success(itemResult.Value);
    }

    public Result RemoveItem(CartItemId itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id.Equals(itemId));

        if (item is null)
            return CartErrors.ItemNotFound;

        _items.Remove(item);

        Touch();

        RaiseDomainEvent(
             CartItemRemovedDomainEvent.Create(
                Id,
                itemId,
                item.CourseId));

        return Result.Success();
    }

    public Result Clear()
    {
        _items.Clear();

        Touch();

        return Result.Success();
    }

    #endregion

    #region User Management

    /// <summary>
    /// Assigns a guest cart to an authenticated user using only Value Objects.
    /// Caller (Application Layer) extracts these from the User aggregate.
    /// </summary>
    public Result AssignToUser(
        UserId userId,
        FullName userFullName,
        Email userEmail)
    {
        if (userId is null)
            return CartErrors.InvalidUser;

        if (userFullName is null)
            return CartErrors.InvalidUserName;

        if (userEmail is null)
            return CartErrors.InvalidUserEmail;

        if (!IsGuest)
            return CartErrors.CartAlreadyAssigned;

        UserId = userId;
        SessionId = null;
        UserFullName = userFullName;
        UserEmail = userEmail;

        Touch();

        RaiseDomainEvent(
             CartAssignedToUserDomainEvent.Create(Id, userId));

        return Result.Success();
    }

    #endregion

    #region Merge

    /// <summary>
    /// Merges items from a guest cart's items into this user cart.
    /// Accepts only the items list — not the entire Cart aggregate.
    /// Caller (Application Layer) fetches the guest cart and passes its items.
    /// </summary>
    public Result MergeGuestItems(
        CartId guestCartId,
        IReadOnlyCollection<CartItem> guestItems)
    {
        if (IsGuest)
            return CartErrors.DestinationCartIsNotUser;

        if (guestCartId is null)
            return CartErrors.InvalidGuestCart;

        if (guestItems is null || guestItems.Count == 0)
            return Result.Success();

        foreach (var guestItem in guestItems)
        {
            // Skip duplicates — keep existing items
            var exists = _items.Any(i => i.CourseId.Equals(guestItem.CourseId));

            if (exists) continue;

            Result<CartItem> newItemResult = CartItem.Create(
                Id,
                guestItem.CourseId,
                guestItem.CourseTitle,
                guestItem.CourseInstructorName,
                guestItem.CourseImageName,
                guestItem.UnitPrice);

            if (newItemResult.IsFailure)
                return newItemResult.Error;

            _items.Add(newItemResult.Value);
        }

        Touch();

        RaiseDomainEvent(
             CartsMergedDomainEvent.Create(Id, guestCartId));

        return Result.Success();
    }

    #endregion

    #region Checkout

    /// <summary>
    /// Validates checkout eligibility for the cart.
    /// Does NOT create Order — that's the Application Layer's responsibility.
    /// Cart only validates its own invariants.
    /// </summary>
    public Result ValidateForCheckout(UserId userId)
    {
        if (_items.Count == 0)
            return CartErrors.CartIsEmpty;

        if (IsGuest)
            return CartErrors.GuestCheckoutNotAllowed;

        // Validate the requesting user matches cart owner
        if (!UserId!.Equals(userId))
            return CartErrors.UserMismatch;

        RaiseDomainEvent(
             CartCheckedOutDomainEvent.Create(Id, UserId));

        return Result.Success();
    }

    #endregion

    #region Queries

    public bool IsEmpty() => _items.Count == 0;

    public bool ContainsCourse(CourseId courseId)
        => _items.Any(i => i.CourseId.Equals(courseId));

    public CartItem? GetItemByCourse(CourseId courseId)
        => _items.FirstOrDefault(i => i.CourseId.Equals(courseId));

    #endregion

    #region Helpers

    private Money CalculateTotal()
    {
        if (_items.Count == 0)
            return Money.Zero();

        var total = Money.Zero();

        foreach (var item in _items)
            total = total.Add(item.UnitPrice).Value;

        return total;
    }

    private void Touch() => MarkAsModified();

    #endregion
}

#region Errors

public static class CartErrors
{
    #region Validation

    public static readonly Error InvalidUser =
        Error.Validation("Cart.InvalidUser", "کاربر نامعتبر است.");

    public static readonly Error InvalidUserName =
        Error.Validation("Cart.InvalidUserName", "نام کاربر نامعتبر است.");

    public static readonly Error InvalidUserEmail =
        Error.Validation("Cart.InvalidUserEmail", "ایمیل کاربر نامعتبر است.");

    public static readonly Error InvalidGuestId =
        Error.Validation("Cart.InvalidGuestId", "شناسه مهمان نامعتبر است.");

    public static readonly Error InvalidGuestCart =
        Error.Validation("Cart.InvalidGuestCart", "سبد مهمان نامعتبر است.");

    public static readonly Error CourseAlreadyInCart =
        Error.Validation("Cart.AlreadyExists", "این دوره قبلاً در سبد خرید وجود دارد.");

    public static readonly Error ItemNotFound =
        Error.NotFound("Cart.ItemNotFound", "آیتم مورد نظر یافت نشد.");

    public static readonly Error CartIsEmpty =
        Error.Validation("Cart.Empty", "سبد خرید خالی است.");

    public static readonly Error CurrencyMismatch =
        Error.Validation("Cart.CurrencyMismatch", "واحد پول آیتم‌ها یکسان نیست.");

    public static readonly Error CartAlreadyAssigned =
        Error.Validation("Cart.AlreadyAssigned", "سبد قبلاً به کاربر اختصاص داده شده است.");

    public static readonly Error SourceCartIsNotGuest =
        Error.Validation("Cart.SourceNotGuest", "فقط سبد مهمان قابل ادغام است.");

    public static readonly Error DestinationCartIsNotUser =
        Error.Validation("Cart.DestinationNotUser", "سبد مقصد باید متعلق به کاربر باشد.");

    public static readonly Error GuestCheckoutNotAllowed =
        Error.Forbidden("Cart.GuestCheckoutNotAllowed", "کاربر مهمان نمی‌تواند پرداخت انجام دهد.");

    public static readonly Error UserMismatch =
        Error.Validation("Cart.UserMismatch", "کاربر با مالک سبد خرید مطابقت ندارد.");

    public static Error InvalidCourse =>
        Error.Validation("Cart.InvalidCourse", "دوره نامعتبر است.");

    #endregion

}

#endregion
