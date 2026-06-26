using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Queries.GetByUserId
{
    public sealed class GetUserWalletQueryHandler(IWalletQueryService repository,IMapper mapper) : IQueryHandler<GetUserWalletQuery, WalletDto>
    {
        public async Task<Result<WalletDto>> Handle(GetUserWalletQuery request, CancellationToken cancellationToken)
        {
            var userId = UserId.CreateFrom(request.UserId);
            if (userId.IsFailure)
                return userId.Error;
            WalletDto? wallet = await repository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (wallet is null)
                return Error.NotFound("GetUserWalletQuery.WalletNotFound", "کیف پولی برای این کاربر یافت نشد.");

            return mapper.Map<WalletDto>(wallet);
        }
    }
}
