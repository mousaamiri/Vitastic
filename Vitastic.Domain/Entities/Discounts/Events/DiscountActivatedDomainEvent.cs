using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Discounts.Enums;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Discounts.Events;

#region DiscountActivatedDomainEvent

public sealed record DiscountActivatedDomainEvent : DomainEvent
{
    public Guid DiscountId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountActivatedDomainEvent(Guid discountId)
    {
        DiscountId = discountId;
    }

    public static DiscountActivatedDomainEvent Create(DiscountId discountId)
        => new(discountId.Value);
}

#endregion

#region DiscountCreatedDomainEvent

public sealed record DiscountCreatedDomainEvent : DomainEvent
{
    public Guid DiscountId { get; init; }
    public string DiscountCode { get; init; }
    public DiscountType DiscountType { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountCreatedDomainEvent(Guid discountId, string discountCode, DiscountType discountType)
    {
        DiscountId = discountId;
        DiscountCode = discountCode;
        DiscountType = discountType;
    }

    public static DiscountCreatedDomainEvent Create(DiscountId discountId, DiscountCode discountCode, DiscountType discountType)
        => new(discountId.Value, discountCode.Value, discountType);
}

#endregion

#region DiscountDeactivatedDomainEvent

public sealed record DiscountDeactivatedDomainEvent : DomainEvent
{
    public Guid DiscountId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountDeactivatedDomainEvent(Guid discountId)
    {
        DiscountId = discountId;
    }

    public static DiscountDeactivatedDomainEvent Create(DiscountId discountId)
        => new(discountId.Value);
}

#endregion

#region DiscountExtendedDomainEvent

public sealed record DiscountExtendedDomainEvent : DomainEvent
{
    public Guid DiscountId { get; init; }
    public DateTimeOffset OldEndDate { get; init; }
    public DateTimeOffset NewEndDate { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountExtendedDomainEvent(Guid discountId, DateTimeOffset oldEndDate, DateTimeOffset newEndDate)
    {
        DiscountId = discountId;
        OldEndDate = oldEndDate;
        NewEndDate = newEndDate;
    }

    public static DiscountExtendedDomainEvent Create(DiscountId discountId, DateTimeOffset oldEndDate, DateTimeOffset newEndDate)
        => new(discountId.Value, oldEndDate, newEndDate);
}

#endregion

#region DiscountUsedDomainEvent

public sealed record DiscountUsedDomainEvent : DomainEvent
{
    public Guid DiscountId { get; init; }
    public Guid UserId { get; init; }
    public int UsedCount { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public DiscountUsedDomainEvent(Guid discountId, Guid userId, int usedCount)
    {
        DiscountId = discountId;
        UserId = userId;
        UsedCount = usedCount;
    }

    public static DiscountUsedDomainEvent Create(DiscountId discountId, UserId userId, int usedCount)
        => new(discountId.Value, userId.Value, usedCount);
}

#endregion
