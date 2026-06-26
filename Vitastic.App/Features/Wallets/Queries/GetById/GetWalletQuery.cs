using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.App.Features.Wallets.Queries.GetById
{
    public sealed record GetWalletQuery(
        Guid WalletId) : IQuery<WalletDto>;
}
