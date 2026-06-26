using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Roles.Queries.GetByName
{
    public sealed record GetRoleByNameQuery(
        string RoleName) : IQuery<RoleDto>;
}
