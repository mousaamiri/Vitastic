using Vitastic.Domain.Shared.Events;

namespace Vitastic.Domain.Shared.Models;

public interface IAggregateRoot
{
    public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
