using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>

    {
        Task<TEntity?> FindAsync(TKey id, CancellationToken cancellation = default);
        Task<bool> IsExistAsync(TKey id, CancellationToken cancellation = default);
        Task DeleteAsync(TKey id, CancellationToken cancellation = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default);
        Task AddAsync(TEntity entity, CancellationToken cancellation = default);
        Task UpdateAsync(TEntity entity, CancellationToken token = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellation = default);
    }

}
