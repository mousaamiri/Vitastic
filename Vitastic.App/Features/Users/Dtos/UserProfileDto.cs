namespace Vitastic.App.Features.Users.Dtos;

public sealed record UserAvatarInfoDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }=string.Empty;
    public string Email { get; init; }=string.Empty;
    public string? FullName { get; init; }
    public string Avatar { get; init; }=string.Empty;
    public List<string> RoleNames { get; set; } = [];
}
