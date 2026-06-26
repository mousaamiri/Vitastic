using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Queries.GetUserTransactions
{
    public sealed class GetUserTransactionQueryHandler
        (IWalletQueryService walletQueryService)
        :IQueryHandler<GetUserTransactionsQuery,PaginatedResult<WalletTransactionDto>>
    {
        public async Task<Result<PaginatedResult<WalletTransactionDto>>> Handle(GetUserTransactionsQuery query, CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            (IReadOnlyList<WalletTransactionDto> items, var total) = await walletQueryService
                .GetPagedUserTransactionsAsync(userIdResult.Value,query.PageNumber, query.PageSize,cancellationToken);
            return  new PaginatedResult<WalletTransactionDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}
