using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.App.Features.Wallets.Queries.GetUserTransactions;

public record GetUserTransactionsQuery(Guid UserId,int PageNumber=1,int PageSize=10):IQuery<PaginatedResult<WalletTransactionDto>>;
