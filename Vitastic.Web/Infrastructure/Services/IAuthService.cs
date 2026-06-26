using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Auth;

namespace Vitastic.Web.Infrastructure.Services;
public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model);
    Task<ApiResponse<Guid>> RegisterAsync(RegisterDto model);
    Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id);
    Task<ApiResponse<UserAvatarInfoDto>> GetAvatarInfoAsync(Guid id);
    Task<ApiResponse<string>> GetUserAvatarAsync(Guid userId);
    Task<ApiResponse> LogoutAsync();
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse> ActivateAsync(ActivateUserDto activateUserDto);
    Task<ApiResponse> ResendActivationLinkAsync(ResendActivationDto dto);
    Task<ApiResponse> ForgetPasswordAsync(ForgetPasswordDto dto);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
}
public class AuthService(IApiClient apiClient) : IAuthService
{
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model)
        => await apiClient.PostAsync<LoginResponseDto>("Users/login", model);

    public async Task<ApiResponse<Guid>> RegisterAsync(RegisterDto model)
        => await apiClient.PostAsync<Guid>("Users/register", model);

    public async Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id)
        => await apiClient.GetAsync<UserDetailDto>($"Users/{id}");

    public async Task<ApiResponse> LogoutAsync()
        => await apiClient.PostAsync("Users/logout");

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        => await apiClient.PostAsync<LoginResponseDto>("Users/refresh", new { refreshToken });

    public async Task<ApiResponse> ActivateAsync(ActivateUserDto activateUserDto)
        => await apiClient.PostAsync<LoginResponseDto>("Users/activation",
            new { activateUserDto.Token,activateUserDto.Email });
    public async Task<ApiResponse> ResendActivationLinkAsync(ResendActivationDto dto)
        => await apiClient.PostAsync<ResendActivationDto>("Users/resend-activation-code", dto);

    public async Task<ApiResponse> ForgetPasswordAsync(ForgetPasswordDto dto)
        => await apiClient.PostAsync<ForgetPasswordDto>("Users/forgot-password", dto);

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
        => await apiClient.PostAsync<ResetPasswordDto>("Users/reset-password", dto);

    public async Task<ApiResponse<UserAvatarInfoDto>> GetAvatarInfoAsync(Guid id)
        => await apiClient.GetAsync<UserAvatarInfoDto>($"Users/{id}/avatar/info");

    public async Task<ApiResponse<string>> GetUserAvatarAsync(Guid userId)
        => await apiClient.GetAsync<string>($"Users/{userId}/avatar/image");
    
}
