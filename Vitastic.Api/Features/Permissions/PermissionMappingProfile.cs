using AutoMapper;
using Vitastic.Api.Features.Permissions.Requests;
using Vitastic.Api.Features.Permissions.Responses;
using Vitastic.App.Features.Permissions.Commands.Create;
using Vitastic.App.Features.Permissions.Commands.Update;
using Vitastic.App.Features.Permissions.Dtos;

namespace Vitastic.Api.Features.Permissions;

public class PermissionMappingProfile: Profile
{
    public PermissionMappingProfile()
    {

        CreateMap<CreatePermissionRequest, CreatePermissionCommand>();
        CreateMap<UpdatePermissionRequest,UpdatePermissionCommand>();

        CreateMap<RolePermissionDto, PermissionResponse>();
    }
}
