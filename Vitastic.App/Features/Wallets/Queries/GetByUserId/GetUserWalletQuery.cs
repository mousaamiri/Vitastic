using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.App.Features.Wallets.Queries.GetByUserId
{
    public sealed record GetUserWalletQuery(
        Guid UserId) : IQuery<WalletDto>;
}
