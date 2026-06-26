namespace Vitastic.Web.Models.DTOs.Auth;

public sealed record UserDetailDto
{
    public UserDetailDto(Guid id, string userName, string email,string? firstName, string? lastName
        , string? phoneNumber, decimal walletBalance, bool isActive,
        DateTimeOffset registerDate,DateTimeOffset? lastLoginDate,
        List<string> roleNames,string? avatar)
    {
        Id = id;
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        WalletBalance = walletBalance;
        IsActive = isActive;
        RegisterDate = registerDate;
        LastLoginDate = lastLoginDate;
        RoleNames = roleNames;
        Avatar = avatar;
    }

    public UserDetailDto() { }

    public Guid Id { get; set; }
    public string UserName{get; set;}
    public string Email{get; set;}
    public string? FirstName{get; set;}
    public string? LastName{get; set;}
    public string? Avatar{get; set;}
    public decimal WalletBalance{get; set;}
    public bool IsActive{get; set;}
    public DateTimeOffset RegisterDate{get; set;}
    public DateTimeOffset? LastLoginDate{get; set;}
    public List<string> RoleNames{get; set;}
    public string? PhoneNumber{get; set;}
}

public sealed record UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Avatar { get; init; } = string.Empty;
    public DateTimeOffset RegisterDate { get; init; }
    public DateTimeOffset? LastLoginDate { get; init; }

    public UserDto() { }

}
