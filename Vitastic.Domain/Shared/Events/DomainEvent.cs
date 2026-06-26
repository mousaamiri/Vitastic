using MediatR;

namespace Vitastic.Domain.Shared.Events;

public interface IDomainEvent:INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
}

public abstract record DomainEvent(Guid Id, DateTimeOffset OccurredOn) : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
