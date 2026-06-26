using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IDiscountRepository : IRepository<Discount, DiscountId>
{
    Task<Discount?> GetByCodeAsync(
        DiscountCode code,
        CancellationToken cancellation = default);

    /// <summary>
    /// Get active discount
    /// </summary>
    Task<IEnumerable<Discount>> GetActiveDiscountsAsync(
        CancellationToken cancellation = default);

    Task<bool> CodeIsExistAsync(DiscountCode code, CancellationToken token=default);
    Task<bool> TitleIsExistAsync(Title title, CancellationToken token=default);
}
