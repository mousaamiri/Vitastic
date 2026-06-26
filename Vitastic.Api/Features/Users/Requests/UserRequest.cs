namespace Vitastic.Api.Features.Users.Requests;

public sealed record ActivateUserRequest(
    string ActivationCode,
    string Email);
public sealed record ChangeEmailRequest(
    string NewEmail);
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword) ;


public sealed record UpsertUserByAdminRequest(
    string UserName,
    string Email,
    string? Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IFormFile? AvatarFile,
    List<Guid> RoleIds,
    bool IsActive = true);

public record SearchUsersRequest(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 10);
public sealed record RegisterUserRequest(
    string UserName,
    string Email,
    string Password,
    string CallbackUrl);

public sealed record LoginUserRequest(
    string Identifier,   // ایمیل یا نام کاربری
    string Password);
public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RemoveRoleFromUserRequest(
    Guid RoleId);
public sealed record UpdateProfileRequest(
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber=null) ;
public sealed record ForgotPasswordRequest(string Email,string CallbackUrl);

public sealed record ResetPasswordRequest(
    string Token,
    string NewPassword);

public sealed record ResendActivationRequest(string Email,string CallbackUrl);
