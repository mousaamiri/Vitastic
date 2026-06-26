using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Roles.Queries.PermissionsList;

public record ListRolePermissionsQuery(Guid RoleId)
    : IQuery<List<RolePermissionDto>>;
