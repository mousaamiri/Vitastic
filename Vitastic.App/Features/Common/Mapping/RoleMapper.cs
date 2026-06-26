using AutoMapper;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.App.Features.Common.Mapping;

/// <summary>
/// Role Entity to DTO Mapping Profile
/// </summary>
public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        // Role → RoleDto
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.PermissionCount,opt=> opt.MapFrom(src => src.RolePermissions.Count));

        // Role → RoleDetailDto
        CreateMap<Role, RoleDetailDto>()
            .ForMember(dest => dest.Permissions, opt=> opt.MapFrom(src => src.RolePermissions));

        // Permission → PermissionDto
        CreateMap<Permission, RolePermissionDto>();
    }
}
