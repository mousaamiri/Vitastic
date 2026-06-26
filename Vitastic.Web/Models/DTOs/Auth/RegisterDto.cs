namespace Vitastic.Web.Models.DTOs.Auth
{
    public sealed record RegisterDto(
        string UserName,
        string Email,
        string Password,
        string callbackUrl);
}
