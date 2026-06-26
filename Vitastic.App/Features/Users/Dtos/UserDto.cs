namespace Vitastic.App.Features.Users.Dtos;

public sealed record UserDto
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

    public UserDto() { }
}
