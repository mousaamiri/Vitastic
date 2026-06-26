using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Wallets.Queries.CheckSufficient
{
    public sealed class CheckSufficientBalanceQueryHandler(IWalletQueryService walletRepository) : IQueryHandler<CheckSufficientBalanceQuery, bool>
    {
        public async Task<Result<bool>> Handle(CheckSufficientBalanceQuery request, CancellationToken cancellationToken)
        {
            var walletId = WalletId.CreateFrom(request.WalletId);
            if (walletId.IsFailure)
                return Result.Failure<bool>(walletId.Error);

            WalletDto? wallet = await walletRepository.GetByIdAsync(
                walletId.Value,
                cancellationToken);

            if (wallet is null)
                return Error.NotFound("CheckSufficientBalanceQuery.WalletNotFound", "کیف پولی با این شناسه یافت نشد.");

            return  wallet.Balance >= request.Amount;
        }
    }
}
