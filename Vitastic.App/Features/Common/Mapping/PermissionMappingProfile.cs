using AutoMapper;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Common.Mapping;

public class PermissionMappingProfile: Profile
{
    public PermissionMappingProfile()
    {
        CreateMap<RolePermissionDto, Permission>().ReverseMap();
    }
}
