using AutoMapper;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Models;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        #region Authentication
        CreateMap<LoginUserModel, LoginDto>()
            .ForMember(dest => dest.Identifier, opt
                => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt
                => opt.MapFrom(src => src.Password));

        CreateMap<RegisterUserModel, RegisterDto>();

        CreateMap<ForgetPasswordUserModel, ForgetPasswordDto>();
        CreateMap<ResetPasswordUserModel, ResetPasswordDto>();
        #endregion


    }
}
