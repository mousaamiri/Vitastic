using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.Events;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.Domain.Entities.Orders;

public class Order : AggregateRoot<OrderId>
{
    #region Constants

    private const int MaxCustomerNoteLength = 500;
    private const int MaxAdminNoteLength = 1000;
    private const int MaxNotesCount = 50;

    #endregion

    #region Properties

    public OrderNumber OrderNumber { get; private set; }
    public UserId UserId { get; private set; }
    public FullName UserFullName { get; private set; }
    public Email UserEmail { get; private set; }

    // Financials
    public Money ItemsTotal { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money ShippingAmount { get; private set; }
    public Money FinalAmount { get; private set; }

    // Status
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }

    // Payment
    public PaymentTransactionId? PaymentTransactionId { get; private set; }
    public PaymentGateway? PaymentGateway { get; private set; }
    public DateTimeOffset? PaymentDate { get; private set; }

    // Discount
    public DiscountId? DiscountId { get; private set; }
    public DiscountCode? DiscountCode { get; private set; }

    // Contact
    public PhoneNumber? PhoneNumber { get; private set; }
    public Address? BillingAddress { get; private set; }
    public Address? ShippingAddress { get; private set; }

    // Quick-access note fields
    public string? CustomerNote { get; private set; }
    public string? AdminNote { get; private set; }

    #endregion

    #region Collections

    // Items
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Structured note history (owned value objects)
    private readonly List<OrderNote> _notes = new();
    public IReadOnlyCollection<OrderNote> Notes => _notes.AsReadOnly();

    #endregion

    #region Constructors

    protected Order()
    {
    } // EF Core

    private Order(
        OrderId id,
        OrderNumber number,
        UserId userId,
        FullName name,
        Email email,
        PhoneNumber? phone
    ) : base(id)
    {
        OrderNumber = number;
        UserId = userId;
        UserFullName = name;
        UserEmail = email;
        PhoneNumber = phone;

        Status = OrderStatus.Pending;
        PaymentMethod = PaymentMethod.Online;

        ItemsTotal = Money.Zero();
        DiscountAmount = Money.Zero();
        TaxAmount = Money.Zero();
        ShippingAmount = Money.Zero();
        FinalAmount = Money.Zero();
    }

    #endregion

    #region Factory

    /// <summary>
    /// Creates a new Order using only Value Objects — no aggregate references cross boundary.
    /// Caller (Application Layer) is responsible for extracting these from User aggregate.
    /// </summary>
    public static Result<Order> Create(
        UserId userId,
        FullName userFullName,
        Email userEmail,
        PhoneNumber? userPhone = null)
    {
        if (userId is null)
            return OrderErrors.InvalidUser;

        if (userFullName is null)
            return OrderErrors.InvalidUserName;

        if (userEmail is null)
            return OrderErrors.InvalidUserEmail;

        var order = new Order(
            OrderId.New(),
            OrderNumber.Generate(),
            userId,
            userFullName,
            userEmail,
            userPhone
        );

        order.RaiseDomainEvent(OrderCreatedDomainEvent.Create(
            order.Id,
            order.UserId,
            order.OrderNumber
        ));

        return Result.Success(order);
    }

    #endregion

    #region Items Management

    /// <summary>
    /// Add item using value objects only — no entity references cross aggregate boundary.
    /// </summary>
    public Result<OrderItem> AddItem(
        CourseId courseId,
        CourseTitle courseTitle,
        CourseImageName? courseImageName,
        FullName instructorFullName,
        Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (courseId is null)
            return OrderErrors.InvalidCourse;

        // Duplicate check
        if (_items.Any(i => i.CourseId.Equals(courseId)))
            return OrderErrors.CourseAlreadyInOrder;

        // Currency consistency check
        if (_items.Count > 0 && !unitPrice.Currency.Equals(ItemsTotal.Currency))
            return OrderErrors.CurrencyMismatch;

        // Create OrderItem from value objects
        var itemResult = OrderItem.Create(
            Id,
            courseId,
            courseTitle,
            courseImageName,
            instructorFullName,
            unitPrice
        );

        if (itemResult.IsFailure)
            return itemResult;

        var item = itemResult.Value;

        _items.Add(item);
        RecalculateAmounts();

        RaiseDomainEvent(OrderItemAddedDomainEvent.Create(Id, item.Id, courseId));

        return Result.Success(item);
    }

    public Result RemoveItem(OrderItemId itemId)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        var item = _items.FirstOrDefault(x => x.Id.Equals(itemId));
        if (item is null)
            return OrderErrors.ItemNotFound;

        _items.Remove(item);
        RecalculateAmounts();

        RaiseDomainEvent(OrderItemRemovedDomainEvent.Create(Id, itemId));
        return Result.Success();
    }

    public Result ClearItems()
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        _items.Clear();
        RecalculateAmounts();

        return Result.Success();
    }

    #endregion

    #region Discount Management

    /// <summary>
    /// Apply discount using only Value Objects and pre-calculated amount.
    /// Discount validation and calculation must happen in Application Layer.
    /// </summary>
    public Result ApplyDiscount(
        DiscountId discountId,
        DiscountCode discountCode,
        Money discountAmount)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (discountId is null)
            return OrderErrors.InvalidDiscount;

        if (discountCode is null)
            return OrderErrors.InvalidDiscountCode;

        if (discountAmount is null || discountAmount.Value <= 0)
            return OrderErrors.DiscountAmountZero;

        DiscountId = discountId;
        DiscountCode = discountCode;
        DiscountAmount = discountAmount;

        RecalculateAmounts();

        RaiseDomainEvent(DiscountAppliedToOrderDomainEvent.Create(
            Id,
            discountId,
            discountAmount
        ));

        return Result.Success();
    }

    public Result RemoveDiscount()
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (DiscountId is null)
            return Result.Success();

        DiscountId = null;
        DiscountCode = null;
        DiscountAmount = Money.Create(0, ItemsTotal.Currency).Value;

        RecalculateAmounts();
        return Result.Success();
    }

    public Result SetTaxAmount(Money taxAmount)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (taxAmount is null)
            return OrderErrors.InvalidTaxAmount;

        if (taxAmount.Value < 0)
            return OrderErrors.TaxAmountNegative;

        // Currency consistency check
        if (!taxAmount.Currency.Equals(GetOrderCurrency()))
            return OrderErrors.CurrencyMismatch;

        TaxAmount = taxAmount;
        RecalculateAmounts();

        return Result.Success();
    }


    /// <summary>
    /// Clear the tax amount (set to zero). Shortcut for SetTaxAmount(Money.Zero()).
    /// </summary>
    public Result ClearTaxAmount()
        => SetTaxAmount(Money.Create(0, GetOrderCurrency()).Value);

    /// <summary>
    /// Set the shipping amount for this order.
    /// Shipping calculation logic should reside in Application Layer or a Domain Service.
    /// This method only accepts a pre-calculated shipping amount.
    /// Shipping cannot be negative.
    /// </summary>
    public Result SetShippingAmount(Money shippingAmount)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (shippingAmount is null)
            return OrderErrors.InvalidShippingAmount;

        if (shippingAmount.Value < 0)
            return OrderErrors.ShippingAmountNegative;

        // Currency consistency check
        if (!shippingAmount.Currency.Equals(GetOrderCurrency()))
            return OrderErrors.CurrencyMismatch;

        ShippingAmount = shippingAmount;
        RecalculateAmounts();

        return Result.Success();
    }

    /// <summary>
    /// Clear the shipping amount (set to zero). Shortcut for SetShippingAmount(Money.Zero()).
    /// </summary>
    public Result ClearShippingAmount()
        => SetShippingAmount(Money.Create(0, GetOrderCurrency()).Value);

    /// <summary>
    /// Set both tax and shipping in one call.
    /// Useful when both are calculated together (e.g., by a pricing domain service).
    /// </summary>
    public Result SetTaxAndShipping(Money taxAmount, Money shippingAmount)
    {
        // Validate both before applying either — atomic operation
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (taxAmount is null)
            return OrderErrors.InvalidTaxAmount;

        if (shippingAmount is null)
            return OrderErrors.InvalidShippingAmount;

        if (taxAmount.Value < 0)
            return OrderErrors.TaxAmountNegative;

        if (shippingAmount.Value < 0)
            return OrderErrors.ShippingAmountNegative;

        var currency = GetOrderCurrency();

        if (!taxAmount.Currency.Equals(currency))
            return OrderErrors.CurrencyMismatch;

        if (!shippingAmount.Currency.Equals(currency))
            return OrderErrors.CurrencyMismatch;

        // Apply both at once — single recalculation
        TaxAmount = taxAmount;
        ShippingAmount = shippingAmount;
        RecalculateAmounts();

        return Result.Success();
    }

    #endregion

    #region Payment Processing

    public Result ProcessPayment(PaymentTransactionId transactionId, PaymentMethod method)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Processing)
            return OrderErrors.CannotProcessPayment;

        if (transactionId is null)
            return OrderErrors.InvalidTransaction;

        if (_items.Count == 0)
            return OrderErrors.EmptyOrder;

        PaymentTransactionId = transactionId;
        PaymentMethod = method;

        PaymentGateway = method switch
        {
            PaymentMethod.Online => PaymentGateway.Zarinpal,
            PaymentMethod.Wallet => PaymentGateway.Wallet,
            _ => throw new Exception("درگاه پرداخت نامعتبر است.")
        };

        PaymentDate = DateTimeOffset.UtcNow;
        Status = OrderStatus.Processing;

        RaiseDomainEvent(OrderPaymentProcessedDomainEvent.Create(
            Id,
            transactionId,
            PaymentGateway
        ));

        return Result.Success();
    }

    /// <summary>
    /// Complete order using only the transaction's ID, status, and orderId — no aggregate reference.
    /// Application Layer fetches the transaction and passes the needed data.
    /// </summary>
    public Result Complete(
        PaymentTransactionId transactionId,
        OrderId? transactionOrderId,
        TransactionStatus transactionStatus)
    {
        if (Status != OrderStatus.Processing)
            return OrderErrors.OnlyProcessingCanComplete;

        if (PaymentTransactionId is null ||
            !PaymentTransactionId.Equals(transactionId) ||
            transactionOrderId is null || // ← Added null check
            !transactionOrderId.Equals(Id))
            return OrderErrors.CannotCompleteWithoutPayment;

        if (transactionStatus != TransactionStatus.Completed)
            return OrderErrors.CannotCompleteWithoutPayment;

        Status = OrderStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;

        foreach (OrderItem item in _items)
            item.GrantAccess();

        RaiseDomainEvent(OrderCompletedDomainEvent.Create(Id, UserId,
            _items.Select(oi => oi.CourseId).ToList()));

        return Result.Success();
    }

    public Result Cancel(string? reason = null)
    {
        if (!CanBeCancelled())
            return OrderErrors.CannotCancel;

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;

        // Add structured cancellation note
        if (!string.IsNullOrWhiteSpace(reason))
            AddNoteInternal(NoteType.Admin, $"دلیل لغو: {reason.Trim()}");

        RaiseDomainEvent(OrderCancelledDomainEvent.Create(Id, UserId, reason));

        return Result.Success();
    }

    public Result Refund(string? reason = null)
    {
        if (Status != OrderStatus.Completed)
            return OrderErrors.OnlyCompletedCanRefund;

        Status = OrderStatus.Refunded;

        // Add structured refund note
        if (!string.IsNullOrWhiteSpace(reason))
            AddNoteInternal(NoteType.Admin, $"دلیل بازپرداخت: {reason.Trim()}");

        foreach (var item in _items)
            item.RevokeAccess();

        RaiseDomainEvent(OrderRefundedDomainEvent.Create(Id, UserId, reason));

        return Result.Success();
    }

    #endregion

    #region Contact Info

    public Result UpdateContactInformation(
        PhoneNumber phone,
        Address billing,
        Address? shipping = null)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (phone is null)
            return OrderErrors.InvalidPhoneNumber;

        PhoneNumber = phone;
        BillingAddress = billing;
        ShippingAddress = shipping;

        return Result.Success();
    }

    #endregion

    #region Update Customer Information

    public Result UpdateCustomerInformation(
        FullName fullName)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        UserFullName = fullName;

        return Result.Success();
    }

    #endregion

    #region Notes Management

    /// <summary>
    /// Set or update the customer note on the order.
    /// Only allowed when order is in Pending status.
    /// Also appends to structured note history.
    /// </summary>
    public Result SetCustomerNote(string? note)
    {
        if (Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        if (note is not null && note.Trim().Length > MaxCustomerNoteLength)
            return OrderErrors.CustomerNoteTooLong;

        string? trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        string? previousNote = CustomerNote;

        CustomerNote = trimmedNote;

        // Track in structured history only if changed
        if (!string.Equals(previousNote, trimmedNote, StringComparison.Ordinal))
        {
            if (trimmedNote is not null)
                AddNoteInternal(NoteType.Customer, trimmedNote);
            else
                AddNoteInternal(NoteType.System, "یادداشت مشتری حذف شد.");
        }

        return Result.Success();
    }

    /// <summary>
    /// Clear the customer note. Shortcut for SetCustomerNote(null).
    /// </summary>
    public Result ClearCustomerNote()
        => SetCustomerNote(null);

    /// <summary>
    /// Set or update the admin note on the order.
    /// Admin notes can be set in any order status.
    /// Also appends to structured note history.
    /// </summary>
    public Result SetAdminNote(string? note)
    {
        if (note is not null && note.Trim().Length > MaxAdminNoteLength)
            return OrderErrors.AdminNoteTooLong;

        string? trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        string? previousNote = AdminNote;

        AdminNote = trimmedNote;

        // Track in structured history only if changed
        if (!string.Equals(previousNote, trimmedNote, StringComparison.Ordinal))
        {
            if (trimmedNote is not null)
                AddNoteInternal(NoteType.Admin, trimmedNote);
            else
                AddNoteInternal(NoteType.System, "یادداشت مدیر حذف شد.");
        }

        return Result.Success();
    }

    /// <summary>
    /// Clear the admin note. Shortcut for SetAdminNote(null).
    /// </summary>
    public Result ClearAdminNote()
        => SetAdminNote(null);

    /// <summary>
    /// Append a structured note to the order history.
    /// Does NOT overwrite CustomerNote or AdminNote quick-access fields.
    /// Used for audit trail / internal logging within the aggregate.
    /// </summary>
    public Result AddNote(string content, NoteType type)
    {
        if (string.IsNullOrWhiteSpace(content))
            return OrderErrors.NoteContentEmpty;

        int maxLength = type == NoteType.Customer
            ? MaxCustomerNoteLength
            : MaxAdminNoteLength;

        if (content.Trim().Length > maxLength)
            return OrderErrors.NoteTooLong;

        if (_notes.Count >= MaxNotesCount)
            return OrderErrors.MaxNotesReached;

        // Customer notes only when order is pending
        if (type == NoteType.Customer && Status != OrderStatus.Pending)
            return OrderErrors.CanOnlyModifyPending;

        AddNoteInternal(type, content.Trim());

        RaiseDomainEvent(OrderNoteAddedDomainEvent.Create(Id, type));

        return Result.Success();
    }

    /// <summary>
    /// Remove a specific note from the history by matching the value object.
    /// Only Admin and System notes can be removed.
    /// Since OrderNote is a Value Object, removal is by equality match.
    /// </summary>
    public Result RemoveNote(OrderNote note)
    {
        if (note is null)
            return OrderErrors.NoteNotFound;

        var existing = _notes.FirstOrDefault(n => n.Equals(note));

        if (existing is null)
            return OrderErrors.NoteNotFound;

        if (existing.Type == NoteType.Customer)
            return OrderErrors.CannotRemoveCustomerNote;

        _notes.Remove(existing);

        return Result.Success();
    }

    /// <summary>
    /// Remove a note by its index in the notes collection.
    /// Useful when multiple notes have same content but different timestamps.
    /// Only Admin and System notes can be removed.
    /// </summary>
    public Result RemoveNoteAt(int index)
    {
        if (index < 0 || index >= _notes.Count)
            return OrderErrors.NoteNotFound;

        var note = _notes[index];

        if (note.Type == NoteType.Customer)
            return OrderErrors.CannotRemoveCustomerNote;

        _notes.RemoveAt(index);

        return Result.Success();
    }

    /// <summary>
    /// Clear all admin/system notes from history.
    /// Customer notes are preserved.
    /// </summary>
    public Result ClearAdminAndSystemNotes()
    {
        _notes.RemoveAll(n => n.Type != NoteType.Customer);

        return Result.Success();
    }

    #endregion

    #region Note Queries

    /// <summary>
    /// Get all notes filtered by type.
    /// </summary>
    public IReadOnlyCollection<OrderNote> GetNotesByType(NoteType type)
        => _notes.Where(n => n.Type == type).ToList().AsReadOnly();

    /// <summary>
    /// Check if order has any notes.
    /// </summary>
    public bool HasNotes() => _notes.Count > 0;

    /// <summary>
    /// Check if order has notes of specific type.
    /// </summary>
    public bool HasNotes(NoteType type) => _notes.Any(n => n.Type == type);

    /// <summary>
    /// Get the latest note of a specific type, or null.
    /// </summary>
    public OrderNote? GetLatestNote(NoteType type)
        => _notes.Where(n => n.Type == type)
            .OrderByDescending(n => n.CreatedAt)
            .FirstOrDefault();

    #endregion

    #region General Queries

    public bool CanBeCancelled() =>
        Status == OrderStatus.Pending || Status == OrderStatus.Processing;

    public bool IsPaid() =>
        Status == OrderStatus.Completed && PaymentDate.HasValue;

    public bool IsEmpty() => _items.Count == 0;

    public bool HasDiscount() => DiscountAmount.Value > 0;

    public bool ContainsCourse(CourseId cartItemCourseId)
        => _items.Any(c => c.CourseId.Equals(cartItemCourseId));

    #endregion

    #region Private Helpers

    private string GetOrderCurrency()
        => ItemsTotal?.Currency ?? "IRT";

    private void RecalculateAmounts()
    {
        // Sum of items
        ItemsTotal = _items.Count == 0
            ? Money.Create(0, "IRT").Value
            : _items
                .Select(i => i.FinalPrice)
                .Aggregate((sum, p) => sum.Add(p).Value);

        // Final = Items + Tax + Shipping - Discount
        FinalAmount = ItemsTotal
            .Add(TaxAmount).Value
            .Add(ShippingAmount).Value
            .Subtract(DiscountAmount).Value;
    }

    /// <summary>
    /// Internal helper — creates and appends note without validation.
    /// Called by public methods that have already validated input.
    /// </summary>
    private void AddNoteInternal(NoteType type, string content)
    {
        var note = OrderNote.Create(Id, type, content);
        if (note.IsFailure)
            throw new Exception(note.Error.Message);
        _notes.Add(note.Value);
    }

    #endregion


    public Result ChangeStatus(OrderStatus newStatus, string? adminNote = null)
    {
        if (Status == newStatus)
            return Result.Success();

        if (!IsTransitionAllowed(Status, newStatus))
            return OrderErrors.InvalidStatusTransition;

        var previousStatus = Status;
        Status = newStatus;

        ApplyStatusEffects(newStatus, adminNote);
        AddStatusChangeNote(previousStatus, newStatus, adminNote);

        RaiseDomainEvent(OrderStatusChangedDomainEvent.Create(Id, previousStatus, newStatus));

        return Result.Success();
    }

    private static bool IsTransitionAllowed(OrderStatus from, OrderStatus to)
    {
        return (from, to) switch
        {
            (OrderStatus.Pending, OrderStatus.Processing or OrderStatus.Cancelled or OrderStatus.Failed) => true,
            (OrderStatus.Processing, OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Failed) => true,
            (OrderStatus.Completed, OrderStatus.Refunded) => true,
            (OrderStatus.Failed, OrderStatus.Pending) => true,

            _ => false
        };
    }

    private void ApplyStatusEffects(OrderStatus newStatus, string? adminNote)
    {
        switch (newStatus)
        {
            case OrderStatus.Completed:
                CompletedAt = DateTimeOffset.UtcNow;
                foreach (var item in _items)
                    item.GrantAccess();
                RaiseDomainEvent(OrderCompletedDomainEvent.Create(Id, UserId, _items.Select(i => i.CourseId).ToList()));
                break;

            case OrderStatus.Cancelled:
                CancelledAt = DateTimeOffset.UtcNow;
                RaiseDomainEvent(OrderCancelledDomainEvent.Create(Id, UserId, adminNote));
                break;

            case OrderStatus.Refunded:
                foreach (var item in _items)
                    item.RevokeAccess();
                RaiseDomainEvent(OrderRefundedDomainEvent.Create(Id, UserId, adminNote));
                break;

            case OrderStatus.Failed:
                RaiseDomainEvent(OrderFailedDomainEvent.Create(Id, UserId));
                break;
        }

        if (!string.IsNullOrWhiteSpace(adminNote))
            SetAdminNote(adminNote);
    }


    private void AddStatusChangeNote(OrderStatus from, OrderStatus to, string? adminNote)
    {
        var note = $"وضعیت از '{GetStatusName(from)}' به '{GetStatusName(to)}' تغییر کرد.";

        if (!string.IsNullOrWhiteSpace(adminNote))
            note += $" یادداشت: {adminNote.Trim()}";

        AddNoteInternal(NoteType.System, note);
    }

    private static string GetStatusName(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "در انتظار",
        OrderStatus.Processing => "در حال پردازش",
        OrderStatus.Completed => "تکمیل شده",
        OrderStatus.Cancelled => "لغو شده",
        OrderStatus.Refunded => "بازپرداخت شده",
        OrderStatus.Failed => "ناموفق",
        _ => status.ToString()
    };
}
