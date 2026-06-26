namespace Vitastic.Web.Models.DTOs.Auth;

public sealed record ResetPasswordDto(
    string Token,
    string NewPassword
);
