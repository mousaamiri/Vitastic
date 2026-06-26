using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class DiscountRepository(ApplicationWriteDbContext context)
    : BaseRepository<Discount, DiscountId>(context), IDiscountRepository
{
    public async Task<Discount?> GetByCodeAsync(DiscountCode code, CancellationToken cancellation = default)
    {
        return await ExecuteAsync(() =>
                Context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == code, cancellation)
            , cancellation);
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync(CancellationToken cancellation = default)
    {
        return await ExecuteAsync(() =>
                Context.Discounts
                    .Where(d => d.IsActive)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync(cancellation)
            , cancellation);
    }

    public async Task<bool> CodeIsExistAsync(DiscountCode code, CancellationToken token = default) =>
        await ExecuteAsync(() =>
                Context.Discounts
                    .AnyAsync(d => d.Code == code, token)
            , token);

    public async Task<bool> TitleIsExistAsync(Title title, CancellationToken token = default)
        => await ExecuteAsync(() =>
                Context.Discounts
                    .AnyAsync(d => d.Title == title, token)
            , token);
}
