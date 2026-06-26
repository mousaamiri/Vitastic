namespace Vitastic.Domain.Shared.Models;

public interface ISoftDeleteEntity
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    Guid? DeletedBy { get; }
}
