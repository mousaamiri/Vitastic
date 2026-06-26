using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Permissions.Queries.List
{
    public class GetPermissionsListQueryHandler(IPermissionQueryService queryService)
        :IQueryHandler<GetPermissionsListQuery,List<RolePermissionDto>>
    {
        public async Task<Result<List<RolePermissionDto>>> Handle(GetPermissionsListQuery query, CancellationToken cancellationToken)
        {
           List<RolePermissionDto>  permissionDtos
                = await queryService.GetPagedAsync(cancellationToken);
            return  permissionDtos;
        }
    }
}
