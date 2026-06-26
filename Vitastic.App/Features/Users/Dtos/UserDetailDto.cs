namespace Vitastic.App.Features.Users.Dtos;

public sealed class UserDetailDto
{
    public Guid Id {get; init;}
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FirstName {get; init;}
    public string? LastName {get; init;}
    public string? PhoneNumber {get; init;}
    public decimal WalletBalance {get; set;}
    public string? Avatar {get; init;}
    public bool IsActive {get; init;}
    public DateTimeOffset RegisterDate{get; init;}
    public DateTimeOffset?  LastLoginDate{get; init;}
    public List<string> RoleNames { get; set; } = [];
}
