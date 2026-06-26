using AutoMapper;
using Vitastic.Api.Features.Users.Requests;
using Vitastic.Api.Features.Users.Responses;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Users.Commands.ActivateUser;
using Vitastic.App.Features.Users.Commands.AssignRoleToUser;
using Vitastic.App.Features.Users.Commands.ChangeEmail;
using Vitastic.App.Features.Users.Commands.ChangePassword;
using Vitastic.App.Features.Users.Commands.CreateByAdmin;
using Vitastic.App.Features.Users.Commands.Register;
using Vitastic.App.Features.Users.Commands.RemoveRoleFromUser;
using Vitastic.App.Features.Users.Commands.UpdateProfile;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.Api.Features.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        //Request
        CreateMap<UpsertUserByAdminRequest, CreateUserByAdminCommand>();
        CreateMap<UpsertUserByAdminRequest, UpsertUserByAdminRequest>();
        CreateMap<RegisterUserRequest, RegisterUserCommand>();
        CreateMap<ActivateUserRequest, ActivateUserCommand>();
        CreateMap<ChangeEmailRequest,ChangeEmailCommand>();
        CreateMap<ChangePasswordRequest, ChangePasswordCommand>();
        CreateMap<RemoveRoleFromUserRequest, RemoveRoleFromUserCommand>();
        CreateMap<UpdateProfileRequest, UpdateProfileCommand>();

        //Response
        CreateMap<UserAvatarInfoDto,UserAvatarInfoResponse>();
        CreateMap<UserDto,UserResponse>();
        CreateMap<UserDetailDto,UserDetailResponse>();
        CreateMap<JwtResult, AuthTokenResponse>();
    }
}
