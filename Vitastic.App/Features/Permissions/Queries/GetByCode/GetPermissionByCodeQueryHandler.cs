using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Queries.GetByCode
{
    public class GetPermissionByCodeQueryHandler(IPermissionQueryService queryService):IQueryHandler<GetPermissionByCodeQuery,RolePermissionDto>
    {
        public async Task<Result<RolePermissionDto>> Handle(GetPermissionByCodeQuery request, CancellationToken cancellationToken)
        {
            RolePermissionDto? permission=  await queryService.GetByCodeAsync(request.Code, cancellationToken);
            if (permission is null)
                return Error.NotFound("GetPermissionByCodeQuery.NotFound",$"مجوزی با کد {request.Code} یافت نشد.");
            return permission;
        }
    }
}