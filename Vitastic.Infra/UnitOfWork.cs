using Microsoft.EntityFrameworkCore.Storage;
using Vitastic.App.Data;
using Vitastic.Infra.Data;

namespace Vitastic.Infra;

internal sealed class UnitOfWork(ApplicationWriteDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellation);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error saving changes to database", ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellation = default)
    {
        _transaction = await dbContext.Database.BeginTransactionAsync(cancellation);
    }
    public async Task CommitAsync(CancellationToken cancellation = default)
    {
        try
        {
            await SaveChangesAsync(cancellation);
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellation);
            }
        }
        catch
        {
            await RollbackAsync(cancellation);
            throw;
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }


    public async Task RollbackAsync(CancellationToken cancellation = default)
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellation);
            }
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    /// <summary>
    /// Clean up resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (dbContext is not null)
        {
            await dbContext.DisposeAsync();
        }
    }
}
