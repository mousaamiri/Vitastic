using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class CategoryRepository(
    ApplicationWriteDbContext dbContext,
    ILogger<CategoryRepository> logger)
    : BaseRepository<Category, CategoryId>(dbContext),
        ICategoryRepository
{
    public async Task<Category?> GetCategoryByName(string name, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            () =>
                dbContext.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name.Value == name, cancellationToken)
            , cancellationToken);
    }

    public async Task<int> GetCountAsync(bool active, bool onlyParents, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(()=>
            Context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive == active)
                .Where(c => !onlyParents || c.ParentCategoryId == null)
                .CountAsync(cancellationToken), cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<Category> categories, CancellationToken ct = default)
    {
        try
        {
            Context.UpdateRange(categories);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return Task.CompletedTask;
    }
}
