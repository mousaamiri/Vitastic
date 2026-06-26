namespace Vitastic.Web.Models.DTOs.Auth
{
    public sealed record ForgetPasswordDto(
        string Email,
        string CallbackUrl
    );
}
