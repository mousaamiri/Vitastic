using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.Helpers;
using Vitastic.Domain.Shared.Models;
using Vitastic.Infra.Outbox;

namespace Vitastic.Infra.Interceptor;

public class ConvertDomainEventToOutboxMessageInterceptor(ILogger<EmailJsonConverter> logger) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        var outboxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                IReadOnlyCollection<DomainEvent> domainEvents = aggregateRoot.DomainEvents.ToList();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonConvert.SerializeObject(domainEvent, jsonSettings),
                OccurredOnUtc = DateTime.UtcNow
            })
            .ToList();

        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
