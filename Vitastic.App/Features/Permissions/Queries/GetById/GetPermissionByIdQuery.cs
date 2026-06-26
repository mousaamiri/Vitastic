using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Permissions.Dtos;

namespace Vitastic.App.Features.Permissions.Queries.GetById
{
    public record GetPermissionByIdQuery(Guid Id):IQuery<RolePermissionDto>;
}