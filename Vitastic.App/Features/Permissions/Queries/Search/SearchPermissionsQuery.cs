using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;

namespace Vitastic.App.Features.Permissions.Queries.Search
{
    public record SearchPermissionsQuery(string SearchTerm,int PageNumber,int PageSize)
        : IQuery<PaginatedResult<RolePermissionDto>>;
}