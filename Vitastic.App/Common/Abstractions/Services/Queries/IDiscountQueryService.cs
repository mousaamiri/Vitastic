using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IDiscountQueryService
{
    Task<DiscountDetailDto?> GetByIdAsync(
        DiscountId discountId,
        CancellationToken cancellation = default);

    Task<DiscountDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellation = default);

    Task<(IReadOnlyList<DiscountDto> items, int total)> GetPagedAsync(int pageNumber, int pageSize,
        bool? onlyIsActive=true, CancellationToken token=default);

    Task<Result<DiscountCalculationDto>> CalculateDiscountAsync(DiscountId discountId, decimal totalOrderAmount,
        Currency currency, CancellationToken token=default);
    Task<Result<bool>> IsValid(DiscountId discountId, CancellationToken token = default);
}
