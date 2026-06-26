using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.List
{
    public class ListRolesQueryHandler(IRoleQueryService roleService) : IQueryHandler<ListRolesQuery, PaginatedResult<RoleDto>>
    {
        public async Task<Result<PaginatedResult<RoleDto>>> Handle(ListRolesQuery query, CancellationToken cancellationToken)
        {
            (IReadOnlyList<RoleDto> items, var total) = await roleService.GetPagedAsync(query.SearchTerm,query.PageNumber, query.PageSize,cancellationToken);
            return  new PaginatedResult<RoleDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}
