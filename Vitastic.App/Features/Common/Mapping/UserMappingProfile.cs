using AutoMapper;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users;

namespace Vitastic.App.Features.Common.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User → UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<UserAvatarUrlResolver, string>(src => src.UserAvatar));

        // User → UserDetailDto
        CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<UserAvatarUrlResolver, string>(src => src.UserAvatar))
            .ForMember(dest => dest.RoleNames, opt
                => opt.Ignore()); // Manually populated in handler

        // User → UserProfileDto
        CreateMap<User, UserAvatarInfoDto>()
            .ForMember(dest => dest.FullName,
                opt
                    => opt.MapFrom(src => src.UserFullName))
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<UserAvatarUrlResolver, string>(src => src.UserAvatar))
            .ForMember(dest => dest.RoleNames, opt
                => opt.Ignore()); // Manually populated in handler
    }
}
public class UserAvatarUrlResolver(IFileUrlService fileUrlService)
    : IMemberValueResolver<User, UserDetailDto, string, string>, IMemberValueResolver<User, UserAvatarInfoDto, string, string>, IMemberValueResolver<User, UserDto, string, string>
{
    public string Resolve(User source, UserDetailDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);

    public string Resolve(User source, UserAvatarInfoDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);

    public string Resolve(User source, UserDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);
}
