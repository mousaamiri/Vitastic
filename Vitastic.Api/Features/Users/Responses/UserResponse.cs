namespace Vitastic.Api.Features.Users.Responses;

public sealed record UserAvatarInfoResponse(
    Guid Id,
    string UserName,
    string Email,
    string? FullName,
    string? Avatar,
    List<string> RoleNames);
public sealed record UserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; }=string.Empty;
    public string Email { get; init; }=string.Empty;
    public string FirstName { get; init; }=string.Empty;
    public string LastName { get; init; }=string.Empty;
    public string PhoneNumber { get; init; }=string.Empty;
    public bool IsActive { get; init; }
    public string Avatar { get; init; }=string.Empty;
    public DateTimeOffset RegisterDate { get; init; }
    public DateTimeOffset?  LastLoginDate { get; init; }

    public UserResponse() { }
}
public sealed record UserDetailResponse(
    Guid Id,
    string UserName,
    string Email,
    string? FirstName,
    string? LastName,
    string? Avatar,
    string? PhoneNumber,
    decimal WalletBalance,
    bool IsActive,
    DateTimeOffset RegisterDate,
    DateTimeOffset?  LastLoginDate,
    List<string> RoleNames);


public sealed record AuthTokenResponse(
    Guid UserId,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    string[] Roles,
    DateTime RefreshTokenExpiresAt);
