using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;

namespace Vitastic.App.Features.Permissions.Queries.List
{
    public record GetPermissionsListQuery
        : IQuery<List<RolePermissionDto>>;
}
