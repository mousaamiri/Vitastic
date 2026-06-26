using AutoMapper;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Users;

namespace Vitastic.App.Features.Common.Mapping;

public class InstructorMappingProfile:Profile
{
    public InstructorMappingProfile()
    {
        // Instructor →  InstructorDto
        CreateMap<Instructor, InstructorDto>()
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<InstructorAvatarUrlResolver, string>(src => src.Avatar))
            ;
        // Instructor →  InstructorDetailDto
        CreateMap<Instructor, InstructorDetailDto>()
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<InstructorAvatarUrlResolver, string>(src => src.Avatar));

    }
}
#region Resolver

public class InstructorAvatarUrlResolver(IFileUrlService fileUrlService)
    : IMemberValueResolver<Instructor, InstructorDto, string, string>, IMemberValueResolver<Instructor, InstructorDetailDto, string, string>
{
    public string Resolve(Instructor source, InstructorDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);

    public string Resolve(Instructor source, InstructorDetailDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);
}

#endregion
