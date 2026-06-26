using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IWalletQueryService
{
    Task<WalletDto?> GetByIdAsync(WalletId walletIdValue, CancellationToken cancellationToken = default);
    Task<WalletDto?> GetByUserIdAsync(UserId userIdValue, CancellationToken cancellationToken = default);
    Task<Result<WalletBalanceDto>> GetWalletBalanceByIdAsync(WalletId walletId, CancellationToken token = default);

    Task<(IReadOnlyList<WalletTransactionDto> items, int total)>
        GetPagedUserTransactionsAsync(UserId userId, int pageNumber, int pageSize, CancellationToken token = default);

    Task<(IReadOnlyList<WalletDto>, int)> SearchUsersWalletsAsync
        (string? searchTerm, int pageNumber = 1, int pageSize = 10, CancellationToken token = default);
}
