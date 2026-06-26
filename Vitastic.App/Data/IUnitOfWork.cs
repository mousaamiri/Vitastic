namespace Vitastic.App.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellation = default);

    Task CommitAsync(CancellationToken cancellation = default);

    Task RollbackAsync(CancellationToken cancellation = default);
}
