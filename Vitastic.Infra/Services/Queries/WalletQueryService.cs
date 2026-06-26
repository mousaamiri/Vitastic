using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class WalletQueryService(
    string connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<WalletQueryService> logger) : IWalletQueryService
{
    public async Task<WalletDto?> GetByIdAsync(WalletId walletIdValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Wallet? wallet =
                await readDbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletIdValue, cancellationToken);
            return mapper.Map<WalletDto>(wallet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByIdAsync");
            throw;
        }
    }

    public async Task<WalletDto?> GetByUserIdAsync(UserId userIdValue, CancellationToken cancellationToken = default)
    {
        try
        {
            Wallet? wallet =
                await readDbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userIdValue, cancellationToken);
            return mapper.Map<WalletDto>(wallet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByUserIdAsync");
            throw;
        }
    }

    public async Task<WalletDto?> GetByUserIdAsync(UserId userIdValue, int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Wallet? wallet =
                await readDbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userIdValue, cancellationToken);
            return mapper.Map<WalletDto>(wallet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByUserIdAsync");
            throw;
        }
    }

    public async Task<Result<WalletBalanceDto>> GetWalletBalanceByIdAsync(WalletId walletId,
        CancellationToken token = default)
    {
        try
        {
            Wallet? wallet = await readDbContext.Wallets
                .FirstOrDefaultAsync(w => w.Id == walletId, token);
            return mapper.Map<WalletBalanceDto>(wallet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetWalletBalanceByIdAsync");
            throw;
        }
    }

    public async Task<(IReadOnlyList<WalletTransactionDto> items, int total)>
        GetPagedUserTransactionsAsync(UserId userId, int pageNumber = 1, int pageSize = 10,
            CancellationToken token = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");
            Wallet? walletId = await readDbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, token);
            if (walletId == null)
                return (new List<WalletTransactionDto>(), 0);
            IQueryable<PaymentTransaction> query = readDbContext.PaymentTransactions
                .Where(t => t.WalletId == walletId.Id)
                .AsNoTracking()
                .AsQueryable();


            var total = await query.CountAsync(token);

            List<WalletTransactionDto> items = await query
                .OrderByDescending(t => t.PaymentInfo.PaidDate)
                .ThenBy(t => t.CompletedDate)
                .ThenBy(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<WalletTransactionDto>(mapper.ConfigurationProvider)
                .ToListAsync(token);

            logger.LogInformation(
                "Listed {Count} wallet transactions - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in get paged user wallet transactions");
            throw;
        }
    }

    public async Task<(IReadOnlyList<WalletDto>, int)> SearchUsersWalletsAsync(string? searchTerm, int pageNumber = 1,
        int pageSize = 10,
        CancellationToken token = default)
    {
        try
        {
            IQueryable<Wallet> usersQuery;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                usersQuery = readDbContext.Wallets;
            }
            else
            {
                var normalizedSearchTerm = $"%{searchTerm}%";

                usersQuery = readDbContext.Wallets
                    .FromSqlRaw("""
                                SELECT * FROM "Wallets" w
                                INNER JOIN "Users" u ON w."UserId" = u."Id"
                                WHERE UPPER("FirstName") LIKE UPPER(@searchTerm) OR
                                      UPPER("LastName") LIKE UPPER(@searchTerm)
                                """
                        , new NpgsqlParameter("@searchTerm", normalizedSearchTerm));
            }

            var totalCount = await usersQuery.CountAsync(token);

            List<Wallet> pagedUsers = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
            List<WalletDto> dtos = mapper.Map<List<WalletDto>>(pagedUsers);
            await MapUserData(dtos);
            return (dtos, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در هنگام جستجوی کاربران با عبارت: {SearchTerm}", searchTerm);
            throw;
        }
    }

    private async Task MapUserData(List<WalletDto> dtos)
    {
        Dictionary<Guid, FullName?> map =
            await readDbContext.Users
            .ToDictionaryAsync(u => u.Id.Value, u => u.UserFullName);

        foreach (WalletDto w in dtos)
            w.UserFullName = map.ContainsKey(w.UserId)? map.GetValueOrDefault<Guid, FullName>(w.UserId) : "";

    }
}
