using System.ComponentModel.DataAnnotations;

namespace Vitastic.Web.Models.DTOs.UserProfile;

public sealed record UpdateProfileRequest(
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null);
public sealed record ChangeEmailRequest(
    string NewEmail);
public record ChangePasswordDto
{
    [Required(ErrorMessage = "رمز عبور فعلی الزامی است.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور جدید الزامی است.")]
    [MinLength(6, ErrorMessage = "رمز عبور جدید باید حداقل 6 کاراکتر باشد.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأیید رمز عبور جدید الزامی است.")]
    [Compare("NewPassword", ErrorMessage = "رمز عبور و تأیید آن یکسان نیستند.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
public sealed record UpsertUserByAdminRequest(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IFormFile? AvatarFile,
    List<Guid> RoleIds,
    bool IsActive = true);
