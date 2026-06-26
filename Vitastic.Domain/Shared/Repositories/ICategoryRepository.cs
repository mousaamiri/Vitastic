using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<Category?> GetCategoryByName(string name,CancellationToken cancellationToken);
    Task<int> GetCountAsync(bool active, bool onlyParents, CancellationToken cancellationToken);
    Task UpdateRangeAsync(IEnumerable<Category> categories, CancellationToken ct=default);
}
