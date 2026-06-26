using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class DiscountQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<DiscountQueryService> logger) : IDiscountQueryService
{
    public async Task<DiscountDetailDto?> GetByIdAsync(
        DiscountId discountId,
        CancellationToken cancellation = default)
    {
        try
        {
            Discount? discount =
                await readDbContext.Discounts.FirstOrDefaultAsync(d => d.Id == discountId, cancellation);

            return discount is null ? null : mapper.Map<DiscountDetailDto>(discount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching discount with ID {DiscountId}", discountId);
            throw;
        }
    }

    public async Task<DiscountDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellation = default)
    {
        try
        {
            Discount? discount = await readDbContext.Discounts
                .FirstOrDefaultAsync(d => d.Code == code, cancellation);

            return discount is null ? null : mapper.Map<DiscountDto>(discount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching discount with code {Code}", code);
            throw;
        }
    }

    public async Task<(IReadOnlyList<DiscountDto> items, int total)>
        GetPagedAsync(int pageNumber, int pageSize, bool? onlyIsActive, CancellationToken token = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Discount> query = readDbContext.Discounts
                .AsNoTracking()
                .AsQueryable();

            if (onlyIsActive.HasValue)
                query = query.Where(t => t.IsActive == onlyIsActive.Value);

            var total = await query.CountAsync(token);

            List<DiscountDto> items = await query
                .OrderByDescending(t => t.Title)
                .ThenBy(t => t.Code)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<DiscountDto>(mapper.ConfigurationProvider)
                .ToListAsync(token);

            logger.LogInformation(
                "Listed {Count} tags - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    public async Task<Result<DiscountCalculationDto>> CalculateDiscountAsync(DiscountId discountId, decimal totalOrderAmount,
        Currency currency,
        CancellationToken token = default)
    {
        try
        {
            Discount discount = await readDbContext.Discounts.FirstAsync(d => d.Id == discountId, token);
            Result<Money> totalMoney = Money.Create(totalOrderAmount);
            if (totalMoney.IsFailure)
                return totalMoney.Error;
            Result<Money> discountAmountResult = discount.CalculateDiscountAmount(totalMoney.Value);
            if (discountAmountResult.IsFailure)
                return discountAmountResult.Error;
            var finalAmount = totalOrderAmount - discountAmountResult.Value.Value;
            return new DiscountCalculationDto(discountId, totalOrderAmount, discountAmountResult.Value.Value,
                finalAmount, currency.Code, true, string.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in calculate discount with id :  {Id} " +
                                "and currency: {Currency} with total order amount {TotalOrderAmount}",
                discountId, currency.Code, totalOrderAmount);
            throw;
        }
    }

    public async Task<Result<bool>> IsValid(DiscountId discountId, CancellationToken token = default)
    {
        try
        {
            return await readDbContext.Discounts
                .AnyAsync(d =>
                        d.Id == discountId
                        && d.IsActive
                        && d.EndDate > DateTimeOffset.UtcNow
                        && d.StartDate < DateTimeOffset.UtcNow
                        && (!d.UsageLimit.HasValue)
                    , token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in validity check discount with  id :  {Id} ",
                discountId);
            throw;
        }
    }
}
