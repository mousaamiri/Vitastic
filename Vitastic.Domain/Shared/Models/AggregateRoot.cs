using Vitastic.Domain.Shared.Events;

namespace Vitastic.Domain.Shared.Models
{
    public abstract class AggregateRoot<TKey>
        : FullEntity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        //----------------------
        // Domain Properties
        //----------------------
        private readonly List<DomainEvent> _domainEvents = [];
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        //----------------------
        // Constructors
        //----------------------
        protected AggregateRoot() { } // For EF
        protected AggregateRoot(TKey id) : base(id) { }
        //----------------------
        // Domain Events
        //----------------------
        protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
