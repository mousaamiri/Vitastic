namespace Vitastic.Domain.Shared.Models;

public interface IAuditableEntity
{
    Guid CreatedBy { get; }
    DateTimeOffset  CreatedAt { get; }
    Guid? UpdatedBy { get; }
    DateTimeOffset? UpdatedAt { get; }
}
