using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.Search
{
    public sealed class SearchUsersQueryHandler
        (IUserQueryService userQueryService)
        : IQueryHandler<SearchUsersQuery,PaginatedResult<UserDto>>
    {
        public async Task<Result<PaginatedResult<UserDto>>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
        {
            (IReadOnlyList<UserDto> items, var total) =
                await userQueryService.SearchAsync(query.SearchTerm, query.PageNumber, query.PageSize,cancellationToken);
            return new PaginatedResult<UserDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}