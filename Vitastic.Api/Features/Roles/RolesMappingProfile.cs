using AutoMapper;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Roles.Requests;
using Vitastic.Api.Features.Roles.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Commands.AddPermissionToRole;
using Vitastic.App.Features.Roles.Commands.Create;
using Vitastic.App.Features.Roles.Commands.UpdateName;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.Api.Features.Roles;

public class RolesMappingProfile:Profile
{
    public RolesMappingProfile()
    {
        CreateMap<RoleDto, RoleResponse>();

        CreateMap<CreateRoleRequest, CreateRoleCommand>();
        CreateMap<UpdateRoleNameRequest, UpdateRoleNameCommand>();
        CreateMap<AddPermissionToRoleRequest,AddPermissionToRoleCommand>();

        CreateMap<RolePermissionDto,RolePermissionResponse>();
        CreateMap<RoleDetailDto, RoleDetailResponse>();

    }
}
