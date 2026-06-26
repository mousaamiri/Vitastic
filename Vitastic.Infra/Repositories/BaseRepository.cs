using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Shared.Exceptions;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Vitastic.Infra.Exceptions;
using Vitastic.Infra.Specifications;

namespace Vitastic.Infra.Repositories;

internal abstract class BaseRepository<TEntity, TKey>(ApplicationWriteDbContext dbContext)
    : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly ApplicationWriteDbContext Context = dbContext;

    // ─────────────────────────────────────────────
    //  CRUD Public Methods - All override ExecuteAsync
    // ─────────────────────────────────────────────

    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellation = default)
        => ExecuteAsync(
            async () =>await Context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellation),
            cancellation);

    public async Task<bool> IsExistAsync(TKey id, CancellationToken cancellation = default)
        => await ExecuteAsync(
            async () =>  await Context.Set<TEntity>().AnyAsync(e=>e.Id.Equals(id), cancellation),
            cancellation);

    public Task DeleteAsync(TKey id, CancellationToken cancellation = default)
        => ExecuteAsync(
            async () =>
            {
                var entity = await Context.Set<TEntity>()
                    .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellation);

                if (entity is not null)
                    Context.Set<TEntity>().Remove(entity);
            },
            cancellation);
  public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default)
        => ExecuteAsync<IEnumerable<TEntity>>(
            async () => await Context.Set<TEntity>().ToListAsync(cancellation),
            cancellation);
    public Task AddAsync(TEntity entity, CancellationToken cancellation = default)
        => ExecuteAsync(
            async () => { await Context.Set<TEntity>().AddAsync(entity, cancellation); },
            cancellation);

    public Task UpdateAsync(TEntity entity, CancellationToken token = default)
        => ExecuteAsync(
            () =>
            {
                Context.Set<TEntity>().Update(entity);
                return Task.CompletedTask;
            },
            token);

    public Task DeleteAsync(TEntity entity, CancellationToken cancellation = default)
        => ExecuteAsync(
            () =>
            {
                Context.Set<TEntity>().Remove(entity);
                return Task.CompletedTask;
            },
            cancellation);

    // ─────────────────────────────────────────────
    //  Specification
    // ─────────────────────────────────────────────

    protected IQueryable<TEntity> ApplySpecification(Specification<TEntity, TKey> specification)
        => SpecificationEvaluator.GetQuery(Context.Set<TEntity>(), specification);

    protected Task<IEnumerable<TEntity>> GetBySpecAsync(
        Specification<TEntity, TKey> specification,
        CancellationToken cancellation = default)
        => ExecuteAsync<IEnumerable<TEntity>>(
            async () => await ApplySpecification(specification).ToListAsync(cancellation),
            cancellation);
// ─────────────────────────────────────────────
    //  Core Exception Wrapper
    // ─────────────────────────────────────────────

    /// <summary>
    /// All DB operations must pass through this wrapper.
    /// Converts DB Exceptions to Domain/Infra Exceptions.
    /// </summary>
    protected async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellation = default)
    {
        try
        {
            return await operation();
        }
        catch (UniqueConstraintException ex)
        {
            // EntityFramework.Exceptions - The most accurate way to detect Unique Violation
            throw new UniqueConstraintViolatedException(
                ex.ConstraintName ?? ExtractPropertyNameFromConstraint(ex),
                null);
        }
        catch (CannotInsertNullException ex)
        {
            // NULL constraint violation
            throw new RepositoryException(
                "Database.NullConstraint",
                $"مقدار خالی برای فیلد اجباری در {typeof(TEntity).Name}",
                ex);
        }
        catch (MaxLengthExceededException ex)
        {
            // Field length exceeded
            throw new RepositoryException(
                "Database.MaxLengthExceeded",
                $"طول مقدار وارد شده از حد مجاز برای {typeof(TEntity).Name} بیشتر است",
                ex);
        }
        catch (NumericOverflowException ex)
        {
            throw new RepositoryException(
                "Database.NumericOverflow",
                $"مقدار عددی از حد مجاز در {typeof(TEntity).Name} بیشتر است",
                ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Two users edited a record at the same time
            throw new ConcurrencyConflictException(typeof(TEntity).Name, ex);
        }
        catch (DbUpdateException ex)
        {
                // Other DB errors that do not have a specific category
                throw new RepositoryException(
                "Database.UpdateError",$"خطا در ذخیره‌سازی {typeof(TEntity).Name}",ex);
        }
        catch (OperationCanceledException)
        {
            // Request canceled - normal, just rethrow
            throw;
        }
        catch (DomainException)
        {
            // Domain Exceptions are passed up intact
            throw;
        }
        catch (InfrastructureException)
        {
            // Nested InfraExceptions go untouched
            throw;
        }
        catch (Exception ex)
        {
            // Any other unexpected error
            throw new RepositoryException(
                "Infrastructure.UnexpectedError",
                $"خطای غیرمنتظره در عملیات {typeof(TEntity).Name}",
                ex);
        }
    }

    /// <summary> /// Overload for operations that return a value /// </summary>
    protected Task ExecuteAsync(
        Func<Task> operation,
        CancellationToken cancellation = default)
        => ExecuteAsync(async () =>
        {
            await operation();
            return true;
        }, cancellation);

    // ─────────────────────────────────────────────
    //  Private Helpers
    // ─────────────────────────────────────────────

    private static string ExtractPropertyNameFromConstraint(UniqueConstraintException ex)
    {
        // Constraint name is like IX_Categories_Name
        // We are trying to extract property name
        var constraintName = ex.ConstraintName ?? "";
        var parts = constraintName.Split('_');
        return parts.Length >= 3 ? parts[^1] : constraintName;
    }

}
