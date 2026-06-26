using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Roles.Queries.GetById
{
    public sealed class GetRoleQueryHandler(IRoleQueryService repository, IMapper mapper)
        : IQueryHandler<GetRoleQuery, RoleDetailDto>
    {
        public async Task<Result<RoleDetailDto>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            Result<RoleId> roleIdResult = RoleId.CreateFrom(request.RoleId);
            if (roleIdResult.IsFailure)
                return roleIdResult.Error;
            return await repository.GetById(roleIdResult.Value, cancellationToken);
        }
    }
}
