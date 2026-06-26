using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Instructors.Events;

#region InstructorActivatedEvent

public sealed record InstructorActivatedEvent : DomainEvent
{
    public Guid InstructorId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorActivatedEvent(Guid instructorId)
    {
        InstructorId = instructorId;
    }

    public static InstructorActivatedEvent Create(InstructorId instructorId)
        => new(instructorId.Value);
}

#endregion

#region InstructorCreatedEvent

public sealed record InstructorCreatedEvent : DomainEvent
{
    public Guid InstructorId { get; init; }
    public string InstructorFullName { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorCreatedEvent(Guid instructorId, string instructorFullName)
    {
        InstructorId = instructorId;
        InstructorFullName = instructorFullName;
    }

    public static InstructorCreatedEvent Create(InstructorId instructorId, FullName fullName)
        => new(instructorId.Value, fullName.Value);
}

#endregion

#region InstructorDeactivatedEvent

public sealed record InstructorDeactivatedEvent : DomainEvent
{
    public Guid InstructorId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorDeactivatedEvent(Guid instructorId)
    {
        InstructorId = instructorId;
    }

    public static InstructorDeactivatedEvent Create(InstructorId instructorId)
        => new(instructorId.Value);
}

#endregion

#region InstructorRejectedEvent

public sealed record InstructorRejectedEvent : DomainEvent
{
    public Guid InstructorId { get; init; }
    public string? Reason { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorRejectedEvent(Guid instructorId, string? reason)
    {
        InstructorId = instructorId;
        Reason = reason;
    }

    public static InstructorRejectedEvent Create(InstructorId instructorId, string? reason)
        => new(instructorId.Value, reason);
}

#endregion

#region InstructorSubmittedForApprovalEvent

public sealed record InstructorSubmittedForApprovalEvent : DomainEvent
{
    public Guid InstructorId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorSubmittedForApprovalEvent(Guid instructorId)
    {
        InstructorId = instructorId;
    }

    public static InstructorSubmittedForApprovalEvent Create(InstructorId instructorId)
        => new(instructorId.Value);
}

#endregion

#region InstructorSuspendedEvent

public sealed record InstructorSuspendedEvent : DomainEvent
{
    public Guid InstructorId { get; init; }
    public string? Reason { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public InstructorSuspendedEvent(Guid instructorId, string? reason)
    {
        InstructorId = instructorId;
        Reason = reason;
    }

    public static InstructorSuspendedEvent Create(InstructorId instructorId, string? reason)
        => new(instructorId.Value, reason);
}

#endregion
