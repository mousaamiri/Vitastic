using System.Net.Http.Headers;
using System.Text;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.DTOs.UserProfile;

namespace Vitastic.Web.Infrastructure.Services;

public interface IUserManagerService
{
    Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id);
    Task<ApiResponse<UserAvatarInfoDto>> GetAvatarInfoAsync(Guid id);
    Task<ApiResponse<string>> GetUserAvatarAsync(Guid userId);
    // Edit
    Task<ApiResponse> EditUserProfile(Guid userId, UpdateProfileRequest request);
    Task<ApiResponse> EditUserEmail(Guid userId, ChangeEmailRequest request);
    Task<ApiResponse> UpdateUserAvatar(Guid userId, IFormFile avatarFile);
    Task<ApiResponse> ChangePassword(ChangePasswordDto dto);
    Task<PaginatedApiResponse<UserDto>> SearchUsersAsync(string searchTerm = "",
        int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<ApiResponse<Guid>> CreateUserByAdmin(UpsertUserByAdminRequest request);
    Task<ApiResponse> UpdateUserByAdmin(UpsertUserByAdminRequest request, Guid userId);
}

internal class UserManagerService(IApiClient apiClient) : IUserManagerService
{
    public async Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id)
        => await apiClient.GetAsync<UserDetailDto>($"Users/{id}");
    public async Task<ApiResponse<UserAvatarInfoDto>> GetAvatarInfoAsync(Guid id)
        => await apiClient.GetAsync<UserAvatarInfoDto>($"Users/{id}/avatar/info");

    public async Task<ApiResponse<string>> GetUserAvatarAsync(Guid userId)
        => await apiClient.GetAsync<string>($"Users/{userId}/avatar/image");

    public async Task<ApiResponse> EditUserProfile(Guid userId, UpdateProfileRequest request)
        => await apiClient.PatchAsync($"Users/{userId}/profile", request);

    public async Task<ApiResponse> EditUserEmail(Guid userId, ChangeEmailRequest request)
        => await apiClient.PatchAsync($"Users/{userId}/email", request);

    public async Task<ApiResponse> UpdateUserAvatar(Guid userId, IFormFile avatarFile)
    {
        var multipartContent = avatarFile.ToMultipartContent(fieldName: "avatarFile");
        return await apiClient.PatchMultipartAsync($"Users/{userId}/avatar", multipartContent);
    }

    public async Task<ApiResponse> ChangePassword(ChangePasswordDto dto)
        => await apiClient.PostAsync($"Users/change-password", dto);

    public async Task<PaginatedApiResponse<UserDto>> SearchUsersAsync(string searchTerm = "", int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        => await apiClient.GetPaginatedAsync<UserDto>($"Users/search", new { searchTerm, pageNumber, pageSize }, ct);

    public async Task<ApiResponse<Guid>> CreateUserByAdmin(UpsertUserByAdminRequest request)
    {
        var additionalFields = new Dictionary<string, string>
        {
            ["userName"] = request.UserName ?? "",
            ["email"] = request.Email ?? "",
            ["password"] = request.Password ?? "", // حتی اگر خالی باشد، ارسال شود
            ["firstName"] = request.FirstName ?? "",
            ["lastName"] = request.LastName ?? "",
            ["phoneNumber"] = request.PhoneNumber ?? "",
            ["isActive"] = request.IsActive.ToString()
        };

        if (request.RoleIds?.Any() == true)
        {
            foreach (var roleId in request.RoleIds)
            {
                additionalFields.Add("roleIds", roleId.ToString());
            }
        }

        MultipartFormDataContent content;
        if (request.AvatarFile is not null)
        {
            content = request.AvatarFile.ToMultipartContent(fieldName: "avatarFile", additionalFields);
        }
        else
        {
            content = new MultipartFormDataContent();
            foreach (var field in additionalFields)
            {
                content.Add(new StringContent(field.Value, Encoding.UTF8), field.Key);
            }
        }

        return await apiClient.PostMultipartAsync<Guid>($"Users/by-admin", content);
    }


    public async Task<ApiResponse> UpdateUserByAdmin(UpsertUserByAdminRequest request, Guid userId)
    {
        var additionalFields = new Dictionary<string, string>
        {
            ["userName"] = request.UserName ?? "",
            ["email"] = request.Email ?? "",
            ["firstName"] = request.FirstName ?? "",
            ["lastName"] = request.LastName ?? "",
            ["phoneNumber"] = request.PhoneNumber ?? "",
            ["isActive"] = request.IsActive.ToString()
        };

        if (!string.IsNullOrEmpty(request.Password))
            additionalFields["password"] = request.Password;

        if (request.RoleIds?.Any() == true)
        {
            foreach (var roleId in request.RoleIds)
            {
                additionalFields.Add("roleIds", roleId.ToString());
            }
        }

        MultipartFormDataContent content;
        if (request.AvatarFile is not null)
        {
            content = request.AvatarFile.ToMultipartContent(fieldName: "avatarFile", additionalFields);
        }
        else
        {
            content = new MultipartFormDataContent();
            foreach (var field in additionalFields)
            {
                content.Add(new StringContent(field.Value, Encoding.UTF8), field.Key);
            }
        }
        return await apiClient.PutMultipartAsync($"Users/{userId}/by-admin", content);
    }
}
