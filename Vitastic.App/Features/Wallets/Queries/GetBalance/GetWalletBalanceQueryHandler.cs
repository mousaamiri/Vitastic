using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Wallets.Queries.GetBalance
{
    public sealed class GetWalletBalanceQueryHandler(IWalletQueryService repository,
        IMapper mapper) : IQueryHandler<GetWalletBalanceQuery, WalletBalanceDto>
    {
        public async Task<Result<WalletBalanceDto>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var walletId = WalletId.CreateFrom(request.WalletId);
            if (walletId.IsFailure)
                return walletId.Error;
            return await repository.GetWalletBalanceByIdAsync(walletId.Value, cancellationToken);
        }
    }
}
