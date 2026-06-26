using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Queries.GetById
{
    public sealed class GetWalletQueryHandler(IWalletQueryService repository,IMapper mapper) : IQueryHandler<GetWalletQuery, WalletDto>
    {
        public async Task<Result<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            var walletId = WalletId.CreateFrom(request.WalletId);
            if (walletId.IsFailure)
                return walletId.Error;
            WalletDto? wallet = await repository.GetByIdAsync(walletId.Value, cancellationToken);
            if (wallet is null)
                return Error.NotFound("GetWalletQuery.WalletNotFound", "کیف پولی با این شناسه یافت نشد.");

            return mapper.Map<WalletDto>(wallet);
        }
    }
}
