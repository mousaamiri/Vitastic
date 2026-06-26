namespace Vitastic.Web.Models.DTOs.Auth
{
    public sealed record UserAvatarInfoDto(
        Guid Id,
        string UserName,
        string Email,
        string? FullName,
        string? Avatar,
        List<string> RoleNames);
}