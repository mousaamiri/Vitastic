using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Permissions.Queries.GetByCode;

public record GetPermissionByCodeQuery(string Code):IQuery<RolePermissionDto>;
