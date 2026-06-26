using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.GetByName
{
    public sealed class GetRoleByNameQueryHandler(IRoleQueryService repository, IMapper mapper) : IQueryHandler<GetRoleByNameQuery, RoleDto>
    {
        public async Task<Result<RoleDto>> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            var roleNameResult = RoleName.Create(request.RoleName);
            if (roleNameResult.IsFailure)
                return roleNameResult.Error;
            return await repository.FindByNameAsync(roleNameResult.Value, cancellationToken);
        }
    }
}
