using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Orders.Events;

#region DiscountAppliedToOrderDomainEvent

public sealed record DiscountAppliedToOrderDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid DiscountId { get; init; }
    public decimal DiscountAmount { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountAppliedToOrderDomainEvent(Guid orderId, Guid discountId, decimal discountAmount)
    {
        OrderId = orderId;
        DiscountId = discountId;
        DiscountAmount = discountAmount;
    }

    public static DiscountAppliedToOrderDomainEvent Create(OrderId orderId, DiscountId discountId, Money discountAmount)
        => new(orderId.Value, discountId.Value, discountAmount.Value);
}

#endregion

#region OrderFailedDomainEvent

public sealed record OrderFailedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderFailedDomainEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }

    public static OrderFailedDomainEvent Create(OrderId orderId, UserId userId)
        => new(orderId.Value, userId.Value);
}

#endregion

#region OrderStatusChangedDomainEvent


public sealed record OrderStatusChangedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public OrderStatus PreviousStatus { get; init; }
    public OrderStatus NewStatus { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderStatusChangedDomainEvent(Guid orderId, OrderStatus previousStatus, OrderStatus newStatus)
    {
        OrderId = orderId;
        PreviousStatus =  previousStatus;
        NewStatus =  newStatus;
    }

    public static OrderStatusChangedDomainEvent Create(OrderId orderId, OrderStatus previousStatus,OrderStatus  newStatus)
        => new(orderId.Value, previousStatus,newStatus);
}

#endregion
#region OrderCancelledDomainEvent

public sealed record OrderCancelledDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string Reason { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderCancelledDomainEvent(Guid orderId, Guid userId, string reason)
    {
        OrderId = orderId;
        UserId = userId;
        Reason = reason;
    }

    public static OrderCancelledDomainEvent Create(OrderId orderId, UserId userId, string reason)
        => new(orderId.Value, userId.Value, reason);
}

#endregion

#region OrderCompletedDomainEvent

public sealed record OrderCompletedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public IReadOnlyList<Guid> OrderCourseIds { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderCompletedDomainEvent(Guid orderId, Guid userId, IReadOnlyList<Guid> orderCourseIds)
    {
        OrderId = orderId;
        UserId = userId;
        OrderCourseIds = orderCourseIds;
    }

    public static OrderCompletedDomainEvent Create(OrderId orderId, UserId userId, IReadOnlyList<CourseId> courseIds)
        => new(orderId.Value, userId.Value, courseIds.Select(c => c.Value).ToList());
}

#endregion

#region OrderCreatedDomainEvent

public sealed record OrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderCreatedDomainEvent(Guid orderId, Guid userId, string orderNumber)
    {
        OrderId = orderId;
        UserId = userId;
        OrderNumber = orderNumber;
    }

    public static OrderCreatedDomainEvent Create(OrderId orderId, UserId userId, OrderNumber orderNumber)
        => new(orderId.Value, userId.Value, orderNumber.Value);
}

#endregion

#region OrderItemAddedDomainEvent

public sealed record OrderItemAddedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid OrderItemId { get; init; }
    public Guid CourseId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderItemAddedDomainEvent(Guid orderId, Guid orderItemId, Guid courseId)
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
        CourseId = courseId;
    }

    public static OrderItemAddedDomainEvent Create(OrderId orderId, OrderItemId orderItemId, CourseId courseId)
        => new(orderId.Value, orderItemId.Value, courseId.Value);
}

#endregion

#region OrderItemRemovedDomainEvent

public sealed record OrderItemRemovedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid OrderItemId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderItemRemovedDomainEvent(Guid orderId, Guid orderItemId)
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
    }

    public static OrderItemRemovedDomainEvent Create(OrderId orderId, OrderItemId orderItemId)
        => new(orderId.Value, orderItemId.Value);
}

#endregion

#region OrderNoteAddedDomainEvent

public sealed record OrderNoteAddedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public NoteType NoteType { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderNoteAddedDomainEvent(Guid orderId, NoteType noteType)
    {
        OrderId = orderId;
        NoteType = noteType;
    }

    public static OrderNoteAddedDomainEvent Create(OrderId orderId, NoteType noteType)
        => new(orderId.Value, noteType);
}

#endregion

#region OrderPaymentProcessedDomainEvent

public sealed record OrderPaymentProcessedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid TransactionId { get; init; }
    public string Gateway { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderPaymentProcessedDomainEvent(Guid orderId, Guid transactionId, string gateway)
    {
        OrderId = orderId;
        TransactionId = transactionId;
        Gateway = gateway;
    }

    public static OrderPaymentProcessedDomainEvent Create(OrderId orderId, PaymentTransactionId transactionId, PaymentGateway gateway)
        => new(orderId.Value, transactionId.Value, gateway.Value);
}

#endregion

#region OrderRefundedDomainEvent

public sealed record OrderRefundedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string Reason { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public OrderRefundedDomainEvent(Guid orderId, Guid userId, string reason)
    {
        OrderId = orderId;
        UserId = userId;
        Reason = reason;
    }

    public static OrderRefundedDomainEvent Create(OrderId orderId, UserId userId, string reason)
        => new(orderId.Value, userId.Value, reason);
}

#endregion
