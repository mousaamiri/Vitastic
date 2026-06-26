using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface ITagRepository : IRepository<Tag, TagId>
{
    Task<Tag?> GetTagByName(string name);
    Task<bool> IsExistByNameAsync(TagName tagName, CancellationToken cancellationToken);
}
