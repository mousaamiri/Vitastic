using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Queries.List;

public sealed class ListDiscountsQueryHandler(IDiscountQueryService discountService,IMapper mapper)
    : IQueryHandler<ListDiscountsQuery, PaginatedResult<DiscountDto>>
{
    public async Task<Result<PaginatedResult<DiscountDto>>> Handle(ListDiscountsQuery query,
        CancellationToken cancellationToken)
    {
        (IReadOnlyList<DiscountDto> items, var total) =
            await discountService.GetPagedAsync(query.PageNumber, query.PageSize,
                true,cancellationToken);
        return  new PaginatedResult<DiscountDto>(items, total, query.PageNumber, query.PageSize);
    }
}
