using System.ComponentModel.DataAnnotations;

namespace Vitastic.Infra.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; init; }
        [StringLength(5000)]
        public string Type { get; init; } = string.Empty;
        [StringLength(5000)]
        public string Content { get; init; } = string.Empty;
        public DateTimeOffset OccurredOnUtc { get; init; }
        public DateTimeOffset?  ProcessedOnUtc { get; set; }
        [StringLength(5000)]
        public string? Error { get; init; }
    }
}
