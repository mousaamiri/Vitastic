using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Queries.GetById
{
    public class GetPermissionByIdQueryHandler(IPermissionQueryService queryService):IQueryHandler<GetPermissionByIdQuery,RolePermissionDto>
    {
        public async Task<Result<RolePermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permissionIdResult = PermissionId.CreateFrom(request.Id);
            if (permissionIdResult.IsFailure)
                return permissionIdResult.Error;
            RolePermissionDto? permission=  await queryService.GetByIdAsync(permissionIdResult.Value, cancellationToken);
            if (permission is null)
                return Error.NotFound("GetPermissionByCodeQuery.NotFound",$"مجوزی با کد {request.Id} یافت نشد.");
            return permission;
        }
    }
}