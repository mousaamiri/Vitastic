using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.Helpers;
using Vitastic.Infra.Data;
using Vitastic.Infra.Outbox;

namespace Vitastic.Infra.BackgroundJobs
{
    [DisallowConcurrentExecution]
    internal class ProcessOutboxMessagesJob(ApplicationWriteDbContext dbContext,
        IPublisher publisher,ILogger<ProcessOutboxMessagesJob> logger)
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            List<OutboxMessage> messages = await dbContext.Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null)
                .Take(20)
                .ToListAsync();
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
               };

            foreach (OutboxMessage outboxMessage in messages)
            {
                try
                {
                    DomainEvent? domainEvent =
                        JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content, jsonSettings);
                    if (domainEvent == null) continue;

                    await publisher.Publish(domainEvent, context.CancellationToken);
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Log error, but don't stop processing other messages
                    logger.LogError(ex, ex.Message);
                    throw;
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
