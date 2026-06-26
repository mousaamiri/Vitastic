using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Events;

namespace Vitastic.Domain.Entities.Cart.Events;

public sealed record CartItemAddedDomainEvent : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid ItemId { get; init; }
    public Guid CourseId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CartItemAddedDomainEvent(Guid cartId, Guid itemId, Guid courseId)
    {
        CartId = cartId;
        ItemId = itemId;
        CourseId = courseId;
    }

    public static CartItemAddedDomainEvent Create(CartId cartId, CartItemId itemId, CourseId courseId)
        => new(cartId.Value, itemId.Value, courseId.Value);
}

public sealed record CartItemRemovedDomainEvent : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid ItemId { get; init; }
    public Guid CourseId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CartItemRemovedDomainEvent(Guid cartId, Guid itemId, Guid courseId)
    {
        CartId = cartId;
        ItemId = itemId;
        CourseId = courseId;
    }

    public static CartItemRemovedDomainEvent Create(CartId cartId, CartItemId itemId, CourseId courseId)
        => new(cartId.Value, itemId.Value, courseId.Value);
}

public sealed record CartsMergedDomainEvent : DomainEvent
{
    public Guid OriginCartId { get; init; }
    public Guid GuestCartId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CartsMergedDomainEvent(Guid originCartId, Guid guestCartId)
    {
        OriginCartId = originCartId;
        GuestCartId = guestCartId;
    }

    public static CartsMergedDomainEvent Create(CartId originCartId, CartId guestCartId)
        => new(originCartId.Value, guestCartId.Value);
}

public sealed record CartCheckedOutDomainEvent : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CartCheckedOutDomainEvent(Guid cartId, Guid userId)
    {
        CartId = cartId;
        UserId = userId;
    }

    public static CartCheckedOutDomainEvent Create(CartId cartId, UserId userId)
        => new(cartId.Value, userId.Value);
}

public sealed record CartAssignedToUserDomainEvent : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CartAssignedToUserDomainEvent(Guid cartId, Guid userId)
    {
        CartId = cartId;
        UserId = userId;
    }

    public static CartAssignedToUserDomainEvent Create(CartId cartId, UserId userId)
        => new(cartId.Value, userId.Value);
}
