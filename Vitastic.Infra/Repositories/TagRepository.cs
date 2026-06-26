using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class TagRepository(ApplicationWriteDbContext dbContext)
    : BaseRepository<Tag, TagId>(dbContext), ITagRepository
{
    public async Task<Tag?> GetTagByName(string name)
        => await ExecuteAsync(async ()=> await Context.Tags.FirstOrDefaultAsync(x => x.Name.Value == name));

    public async Task<bool> IsExistByNameAsync(TagName tagName, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => await Context.Tags.AnyAsync(x => x.Name == tagName, cancellationToken),cancellationToken);
}
