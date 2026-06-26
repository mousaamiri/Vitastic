using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Roles.Queries.GetById
{
    public sealed record GetRoleQuery(
        Guid RoleId) : IQuery<RoleDetailDto>;
}
