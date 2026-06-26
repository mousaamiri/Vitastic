using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Queries.Search
{
    public class SearchPermissionsQueryHandler(IPermissionQueryService queryService):IQueryHandler<SearchPermissionsQuery,PaginatedResult<RolePermissionDto>>
    {
        public async Task<Result<PaginatedResult<RolePermissionDto>>> Handle(SearchPermissionsQuery query, CancellationToken cancellationToken)
        {
            (IReadOnlyList<RolePermissionDto> items, var total) = await queryService.SearchAsync(query.SearchTerm,query.PageNumber, query.PageSize,cancellationToken);
            return  new PaginatedResult<RolePermissionDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}